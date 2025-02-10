// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

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
    /// Service implementation for interacting with QwenAI's API endpoints.
    /// Handles model calls, token counting, and cost calculations specific to QwenAI models.
    /// </summary>
    public class QwenAIService : IAIProviderService
    {
        private readonly IConfiguration _configuration;
        private readonly AIEndpointsConfig _endpointsConfig;
        private readonly ILogger<QwenAIService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="QwenAIService"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration.</param>
        /// <param name="endpointsConfig">AIEndpointsConfig.</param>
        /// <param name="logger">ILogger.</param>
        public QwenAIService(IConfiguration configuration, IOptions<AIEndpointsConfig> endpointsConfig, ILogger<QwenAIService> logger)
        {
            _configuration = configuration;
            _endpointsConfig = endpointsConfig.Value;
            _logger = logger;
        }

        /// <summary>
        /// Calls QwenAI's API with the specified model and prompt to generate a response.
        /// </summary>
        /// <param name="modelName">The name of the QwenAI model to use (e.g., "qwen-turbo", "qwen-plus").</param>
        /// <param name="prompt">The input prompt to send to the model.</param>
        /// <returns>A ProviderResult containing the model's response, token usage, calculated cost, and time taken.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
        /// <exception cref="ArgumentException">Thrown when the model name is invalid or unsupported.</exception>
        public async Task<ProviderResult> CallModelAsync(string modelName, string prompt)
        {
            try
            {
                // Validate supported models
                if (modelName is not "qwen-turbo" and not "qwen-plus" and not "qwen-max")
                {
                    throw new ArgumentException($"Unsupported model: {modelName}");
                }

                // Get API key from configuration/environment
                string apiKey = _configuration["APIKeys:QwenAI"] ?? throw new KeyNotFoundException("Error: QwenAI API Key is missing");

                var stopwatch = Stopwatch.StartNew();

                // Prepare the request body
                var requestBody = new
                {
                    model = modelName,
                    messages = new[]
                    {
                    new { role = "user", content = prompt },
                    },
                    temperature = 0.7,
                    max_tokens = 2000,  // QwenAI specific parameter
                };

                // Serialize request body to JSON
                var jsonRequest = JsonConvert.SerializeObject(requestBody);
                using var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Create and configure HttpClient
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                // Add any QwenAI specific headers if required
                httpClient.DefaultRequestHeaders.Add("X-QwenAI-Version", "2024-02");  // Example version header

                // Send POST request
                string providerBaseUrl = _endpointsConfig.Providers["QwenAI"].BaseUrl
                    ?? throw new KeyNotFoundException("QwenAI base URL not configured");
                string endpoint = _endpointsConfig.Providers["QwenAI"].Endpoints["chat"]
                    ?? throw new KeyNotFoundException("QwenAI chat endpoint not configured");

                using var response = await httpClient.PostAsync($"{providerBaseUrl}{endpoint}", httpContent);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"QwenAI API request failed with status code {response.StatusCode}: {errorContent}");
                }

                // Read and deserialize the API response
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var completionResponse = JsonConvert.DeserializeObject<QwenAICompletionResponse>(jsonResponse)
                    ?? new QwenAICompletionResponse();
                stopwatch.Stop();

                if (completionResponse?.Choices == null || completionResponse.Choices.Count == 0)
                {
                    throw new InvalidOperationException("Failed to parse a valid response from QwenAI API.");
                }

                // Extract token usage data
                int promptTokens = completionResponse.Usage?.PromptTokens ?? 0;
                int completionTokens = completionResponse.Usage?.CompletionTokens ?? 0;
                int totalTokens = completionResponse.Usage?.TotalTokens ?? (promptTokens + completionTokens);

                // Calculate cost based on QwenAI's pricing
                decimal cost = 0m;
                switch (modelName)
                {
                    case "qwen-turbo":
                        // Example pricing: $0.003 per 1K tokens
                        cost = totalTokens / 1000m * 0.003m;
                        break;
                    case "qwen-plus":
                        // Example pricing: $0.006 per 1K tokens
                        cost = totalTokens / 1000m * 0.006m;
                        break;
                    case "qwen-max":
                        // Example pricing: $0.012 per 1K tokens
                        cost = totalTokens / 1000m * 0.012m;
                        break;
                    default:
                        break;
                }

                // Prepare the provider result
                var providerResult = new ProviderResult
                {
                    Message = completionResponse.Choices[0].Message.Content,
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
                _logger.LogError(ex, "Error: config keys missing for QwenAI Service");
                return new ProviderResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in QwenAI service.");
                return new ProviderResult();
            }
        }
    }
}
