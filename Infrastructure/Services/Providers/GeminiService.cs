// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Infrastructure.Services.Providers
{
    using System.Diagnostics;
    using System.Text;
    using ChatbotBenchmarkAPI.Business.Validation;
    using ChatbotBenchmarkAPI.Features.Compare;
    using ChatbotBenchmarkAPI.Infrastructure.Services.Interfaces;
    using ChatbotBenchmarkAPI.Models.CompletionResponses;
    using ChatbotBenchmarkAPI.Models.Configurations;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// Service implementation for interacting with Google's Gemini API.
    /// Handles model calls, token counting, and cost calculations.
    /// </summary>
    public class GeminiService : IAIProviderService
    {
        private readonly IConfiguration _configuration;
        private readonly AIEndpointsConfig _endpointsConfig;
        private readonly ILogger<GeminiService> _logger;
        private readonly AIModelValidator _modelValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeminiService"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration.</param>
        /// <param name="endpointsConfig">AIEndpointsConfig.</param>
        /// <param name="logger">ILogger.</param>
        /// <param name="modelValidator">The model validation service.</param>
        public GeminiService(
            IConfiguration configuration,
            IOptions<AIEndpointsConfig> endpointsConfig,
            ILogger<GeminiService> logger,
            AIModelValidator modelValidator)
        {
            _configuration = configuration;
            _endpointsConfig = endpointsConfig.Value;
            _logger = logger;
            _modelValidator = modelValidator;
        }

        /// <summary>
        /// Calls Google's Gemini API with the specified model and prompt to generate a response.
        /// </summary>
        /// <param name="modelName">The name of the Anthropic model to use (e.g., "gemini-pro", "claude-3-sonnet-20240229").</param>
        /// <param name="prompt">The input prompt to send to the model.</param>
        /// <returns>A ProviderResult containing the model's response, token usage, calculated cost, and time taken.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
        /// <exception cref="ArgumentException">Thrown when the model name is invalid or unsupported.</exception>
        public async Task<ProviderResult> CallModelAsync(string modelName, string prompt)
        {
            try
            {
                // Validate supported models
                if (!_modelValidator.IsModelSupported("Google", modelName))
                {
                    throw new ArgumentException($"Unsupported model: {modelName}");
                }

                // Get API key from configuration
                string apiKey = _configuration["APIKeys:Google"]
                    ?? throw new KeyNotFoundException("Error: Google API key is missing");

                var stopwatch = Stopwatch.StartNew();

                // Build Gemini API request
                var requestBody = new
                {
                    contents = new[]
                    {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt },
                        },
                    },
                    },
                };

                // Serialize request
                string jsonRequest = JsonConvert.SerializeObject(requestBody);
                using var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Configure HTTP client
                using var httpClient = new HttpClient();

                // Get endpoint configuration
                string baseUrl = _endpointsConfig.Providers["Google"].BaseUrl
                    ?? throw new KeyNotFoundException("Google base URL not configured");
                string endpoint = _endpointsConfig.Providers["Google"].Endpoints["chat"]
                    ?? throw new KeyNotFoundException("Chat endpoint not configured");

                // Send request to Gemini API
                string requestUrl = $"{baseUrl}{endpoint}{modelName}:generateContent?key={apiKey}";
                using var response = await httpClient.PostAsync(requestUrl, httpContent);

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException(
                        $"Gemini API request failed with status code {response.StatusCode}: {errorContent}");
                }

                // Parse response
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var completionResponse = JsonConvert.DeserializeObject<GeminiCompletionResponse>(jsonResponse);

                // Validate response structure
                if (completionResponse?.Candidates == null ||
                    completionResponse.Candidates.Count == 0 ||
                    completionResponse.Candidates[0].Content?.Parts == null ||
                    completionResponse.Candidates[0].Content.Parts.Count == 0)
                {
                    throw new InvalidOperationException("Invalid response from Gemini API");
                }

                stopwatch.Stop();

                // Calculate tokens and cost
                int promptTokens = completionResponse.UsageMetadata?.PromptTokenCount ?? 0;
                int completionTokens = completionResponse.UsageMetadata?.CandidatesTokenCount ?? 0;
                int totalTokens = completionResponse.UsageMetadata?.TotalTokenCount
                    ?? (promptTokens + completionTokens);

                // Cost calculation (adjust pricing as needed)
                decimal cost = modelName switch
                {
                    "gemini-1.5-pro" => (promptTokens / 1000m * 0.00125m) + (completionTokens / 1000m * 0.005m),
                    "gemini-2.0-flash" => (promptTokens / 1000m * 0.0001m) + (completionTokens / 1000m * 0.0004m),
                    "gemini-2.0-pro-exp-02-05" => 0m,
                    _ => 0m
                };

                return new ProviderResult
                {
                    Message = completionResponse.Candidates[0].Content.Parts[0].Text,
                    TotalTokens = totalTokens,
                    Cost = cost,
                    TimeTakenMs = stopwatch.ElapsedMilliseconds,
                };
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, ex.Message);
                return new ProviderResult();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Error: config keys missing for Gemini Service");
                return new ProviderResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Gemini service: {Message}", ex.Message);
                return new ProviderResult();
            }
        }
    }
}
