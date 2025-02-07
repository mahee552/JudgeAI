namespace ChatbotBenchmarkAPI.Infrastructure.Services.Providers
{
    using System.Diagnostics;
    using System.Net.Http.Headers;
    using System.Text;
    using ChatbotBenchmarkAPI.Features.Compare;
    using ChatbotBenchmarkAPI.Infrastructure.Services.Interfaces;
    using ChatbotBenchmarkAPI.Models.CompletionResponses;
    using ChatbotBenchmarkAPI.Models.Configurations;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// Provides service implementation for interacting with the Anthropic AI provider.
    /// </summary>
    /// <remarks>
    /// This service handles communication with the Anthropic API, including authentication,
    /// request formatting, and response processing for AI-generated content.
    /// </remarks>
    /// <seealso cref="IAIProviderService"/>
    public class AnthropicService : IAIProviderService
    {
        private readonly IConfiguration _configuration;
        private readonly AIEndpointsConfig _endpointsConfig;
        private readonly ILogger<AnthropicService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnthropicService"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration.</param>
        /// <param name="endpointsConfig">AIEndpointsConfig.</param>
        /// <param name="logger">ILogger.</param>
        public AnthropicService(IConfiguration configuration, IOptions<AIEndpointsConfig> endpointsConfig, ILogger<AnthropicService> logger)
        {
            _configuration = configuration;
            _endpointsConfig = endpointsConfig.Value;
            _logger = logger;
        }

        /// <summary>
        /// Calls Anthropic's API with the specified model and prompt to generate a response.
        /// </summary>
        /// <param name="modelName">The name of the Anthropic model to use (e.g., "claude-3-opus-20240229", "claude-3-sonnet-20240229").</param>
        /// <param name="prompt">The input prompt to send to the model.</param>
        /// <returns>A ProviderResult containing the model's response, token usage, calculated cost, and time taken.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
        /// <exception cref="ArgumentException">Thrown when the model name is invalid or unsupported.</exception>
        public async Task<ProviderResult> CallModelAsync(string modelName, string prompt)
        {
            try
            {
                // Validate supported models
                if (modelName is not "claude-3-opus-20240229" and not "claude-3-sonnet-20240229" and not "claude-3-haiku-20240307")
                {
                    throw new ArgumentException($"Unsupported model: {modelName}");
                }

                // Get API key from configuration/environment
                string apiKey = _configuration["APIKeys:Anthropic"] ?? throw new KeyNotFoundException("Error: Anthropic API Key is missing");

                var stopwatch = Stopwatch.StartNew();

                // Prepare the request body (Anthropic-specific structure)
                var requestBody = new
                {
                    model = modelName,
                    max_tokens = 4096,
                    messages = new[]
                    {
                    new { role = "user", content = prompt },
                    },
                    temperature = 0.7,
                };

                // Serialize request body to JSON
                var jsonRequest = JsonConvert.SerializeObject(requestBody);
                using var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Create and configure HttpClient
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

                // Send POST request to Anthropic endpoint
                string providerBaseUrl = _endpointsConfig.Providers["Anthropic"].BaseUrl
                    ?? throw new KeyNotFoundException("Anthropic base URL not configured");
                string endpoint = _endpointsConfig.Providers["Anthropic"].Endpoints["chat"]
                    ?? throw new KeyNotFoundException("Anthropic chat endpoint not configured");

                using var response = await httpClient.PostAsync($"{providerBaseUrl}{endpoint}", httpContent);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Anthropic API request failed with status code {response.StatusCode}: {errorContent}");
                }

                // Read and deserialize the API response
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var completionResponse = JsonConvert.DeserializeObject<AnthropicCompletionResponse>(jsonResponse)
                    ?? new AnthropicCompletionResponse();
                stopwatch.Stop();

                if (completionResponse?.Content == null || completionResponse.Content.Count == 0)
                {
                    throw new InvalidOperationException("Failed to parse a valid response from Anthropic API.");
                }

                // Extract token usage data (Anthropic-specific token counting)
                int inputTokens = completionResponse.InputTokens ?? 0;
                int outputTokens = completionResponse.OutputTokens ?? 0;
                int totalTokens = inputTokens + outputTokens;

                // Calculate cost based on Anthropic's pricing
                decimal cost = 0m;
                if (modelName == "claude-3-haiku-20240307")
                {
                    // Pricing: $0.00025 per input 1K tokens, $0.00125 per output 1K tokens
                    cost = (inputTokens / 1000m * 0.00025m) + (outputTokens / 1000m * 0.00125m);
                }
                else if (modelName == "claude-3-sonnet-20240229")
                {
                    // Pricing: $0.003 per input 1K tokens, $0.015 per output 1K tokens
                    cost = (inputTokens / 1000m * 0.003m) + (outputTokens / 1000m * 0.015m);
                }
                else if (modelName == "claude-3-opus-20240229")
                {
                    // Pricing: $0.015 per input 1K tokens, $0.075 per output 1K tokens
                    cost = (inputTokens / 1000m * 0.015m) + (outputTokens / 1000m * 0.075m);
                }

                // Prepare the provider result
                var providerResult = new ProviderResult
                {
                    Message = completionResponse.Content[0].Text,
                    TotalTokens = totalTokens,
                    Cost = cost,
                    TimeTakenMs = stopwatch.ElapsedMilliseconds,
                };

                return providerResult;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, ex.Message);
                return new ProviderResult();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Error: config keys missing for Anthropic Service");
                return new ProviderResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Anthropic service.");
                return new ProviderResult();
            }
        }
    }
}
