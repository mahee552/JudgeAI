// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Infrastructure.Services.Providers
{
    using System.Diagnostics;
    using System.Net.Http.Headers;
    using System.Text;
    using ChatbotBenchmarkAPI.Business.Formatters;
    using ChatbotBenchmarkAPI.Business.Pricing;
    using ChatbotBenchmarkAPI.Business.Validation;
    using ChatbotBenchmarkAPI.Features.Compare;
    using ChatbotBenchmarkAPI.Infrastructure.Services.Interfaces;
    using ChatbotBenchmarkAPI.Models.CompletionResponses;
    using ChatbotBenchmarkAPI.Models.Configurations.Endpoints;
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
        private readonly AIModelValidator _modelValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnthropicService"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration.</param>
        /// <param name="endpointsConfig">AIEndpointsConfig.</param>
        /// <param name="modelValidator">The model validation service.</param>
        public AnthropicService(IConfiguration configuration, IOptions<AIEndpointsConfig> endpointsConfig, AIModelValidator modelValidator)
        {
            _configuration = configuration;
            _endpointsConfig = endpointsConfig.Value;
            _modelValidator = modelValidator;
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
            var stopwatch = new Stopwatch();

            try
            {
                // Validate supported models
                if (!_modelValidator.IsModelSupported("Anthropic", modelName))
                {
                    throw new ArgumentException($"Unsupported model: {modelName}");
                }

                // Get API key from configuration/environment
                string apiKey = _configuration["APIKeys:Anthropic"] ?? throw new KeyNotFoundException("Error: Anthropic API Key is missing");

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
                string endpoint = _endpointsConfig.Providers["Anthropic"].Endpoints["messages"]
                    ?? throw new KeyNotFoundException("Anthropic chat endpoint not configured");

                stopwatch = Stopwatch.StartNew();
                using var response = await httpClient.PostAsync($"{providerBaseUrl}{endpoint}", httpContent);
                stopwatch.Stop();

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Anthropic API request failed with status code {response.StatusCode}: {errorContent}");
                }

                // Read and deserialize the API response
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var completionResponse = JsonConvert.DeserializeObject<AnthropicCompletionResponse>(jsonResponse)
                    ?? new AnthropicCompletionResponse();

                if (completionResponse?.Content == null || completionResponse.Content.Count == 0)
                {
                    throw new InvalidOperationException("Failed to parse a valid response from Anthropic API.");
                }

                // Extract token usage data (Anthropic-specific token counting)
                int inputTokens = completionResponse.InputTokens ?? 0;
                int outputTokens = completionResponse.OutputTokens ?? 0;
                int totalTokens = inputTokens + outputTokens;

                // Prepare the provider result
                var providerResult = new ProviderResult
                {
                    Message = completionResponse.Content[0].Text,
                    TotalTokens = totalTokens,
                    Cost = PricingService.CalculateCost("Anthropic", modelName, inputTokens, outputTokens),
                    TimeTaken = ElapsedTimeFormatter.FormatElapsedTime(stopwatch),
                };

                return providerResult;
            }
            finally
            {
                // In case of exception, stop the watch.
                stopwatch.Stop();
            }
        }
    }
}
