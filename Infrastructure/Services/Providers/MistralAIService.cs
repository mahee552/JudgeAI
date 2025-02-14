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
    /// Service implementation for interacting with Mistral AI's API endpoints.
    /// Handles model calls, token counting, and cost calculations specific to Mistral AI models.
    /// </summary>
    public class MistralAIService : IAIProviderService
    {
        private readonly IConfiguration _configuration;
        private readonly AIEndpointsConfig _endpointsConfig;
        private readonly ILogger<MistralAIService> _logger;
        private readonly AIModelValidator _modelValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="MistralAIService"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration.</param>
        /// <param name="endpointsConfig">AIEndpointsConfig.</param>
        /// <param name="logger">ILogger.</param>
        /// <param name="modelValidator">The model validation service.</param>
        public MistralAIService(IConfiguration configuration, IOptions<AIEndpointsConfig> endpointsConfig, ILogger<MistralAIService> logger, AIModelValidator modelValidator)
        {
            _configuration = configuration;
            _endpointsConfig = endpointsConfig.Value;
            _logger = logger;
            _modelValidator = modelValidator;
        }

        /// <summary>
        /// Calls Mistral AI's API with the specified model and prompt to generate a response.
        /// </summary>
        /// <param name="modelName">The name of the Mistral AI model to use (e.g., "mistral-7b").</param>
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
                if (!_modelValidator.IsModelSupported("MistralAI", modelName))
                {
                    throw new ArgumentException($"Unsupported model: {modelName}");
                }

                // Get API key from configuration/environment
                string apiKey = _configuration["APIKeys:MistralAI"] ?? throw new KeyNotFoundException("Error: API Key is missing");

                // Prepare the request body
                var requestBody = new
                {
                    model = modelName,
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

                // Send POST request
                string providerBaseUrl = _endpointsConfig.Providers["MistralAI"].BaseUrl ?? throw new KeyNotFoundException();
                string endpoint = _endpointsConfig.Providers["MistralAI"].Endpoints["generate"] ?? throw new KeyNotFoundException();

                stopwatch = Stopwatch.StartNew();
                using var response = await httpClient.PostAsync($"{providerBaseUrl}{endpoint}", httpContent);
                stopwatch.Stop();

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Mistral AI API request failed with status code {response.StatusCode}: {errorContent}");
                }

                // Read and deserialize the API response
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var completionResponse = JsonConvert.DeserializeObject<MistralAICompletionResponse>(jsonResponse) ?? new MistralAICompletionResponse();

                if (completionResponse == null || completionResponse.Choices == null || completionResponse.Choices.Count <= 0)
                {
                    throw new InvalidOperationException("Failed to parse a valid response from Mistral AI API.");
                }

                // Extract token usage data (if available)
                int promptTokens = completionResponse.Usage?.PromptTokens ?? 0;
                int completionTokens = completionResponse.Usage?.CompletionTokens ?? 0;
                int totalTokens = completionResponse.Usage?.TotalTokens ?? (promptTokens + completionTokens);

                // Prepare the provider result
                var providerResult = new ProviderResult
                {
                    Message = completionResponse.Choices[0].Message.Content,
                    TotalTokens = totalTokens,
                    Cost = PricingService.CalculateCost("MistralAI", modelName, promptTokens, completionTokens),
                    TimeTaken = ElapsedTimeFormatter.FormatElapsedTime(stopwatch),
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
                _logger.LogError(ex, "Error: config keys missing for Mistral AI Service");
                return new ProviderResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Mistral AI service.");
                return new ProviderResult();
            }
            finally
            {
                stopwatch.Stop();
            }
        }
    }
}
