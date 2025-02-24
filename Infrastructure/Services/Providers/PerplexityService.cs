// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Infrastructure.Services.Providers
{
    using System.Diagnostics;
    using System.Net.Http.Headers;
    using System.Text;
    using ChatbotBenchmarkAPI.Business.Pricing;
    using ChatbotBenchmarkAPI.Business.Validation.ModelValidation;
    using ChatbotBenchmarkAPI.Exceptions;
    using ChatbotBenchmarkAPI.Infrastructure.Services.Interfaces;
    using ChatbotBenchmarkAPI.Models.CompletionResponses;
    using ChatbotBenchmarkAPI.Models.Configurations.Endpoints;
    using ChatbotBenchmarkAPI.Models.Request;
    using ChatbotBenchmarkAPI.Models.Response;
    using ChatbotBenchmarkAPI.Utilities.Builders;
    using ChatbotBenchmarkAPI.Utilities.Formatters;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// Service implementation for interacting with Perplexity's API endpoints.
    /// Handles model calls, token counting, and cost calculations specific to Perplexity models.
    /// </summary>
    public class PerplexityService : IAIProviderService
    {
        private readonly IConfiguration _configuration;
        private readonly AIEndpointsConfig _endpointsConfig;
        private readonly AIModelValidator _modelValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerplexityService"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration.</param>
        /// <param name="endpointsConfig">AIEndpointsConfig.</param>
        /// <param name="modelValidator">The model validation service.</param>
        public PerplexityService(IConfiguration configuration, IOptions<AIEndpointsConfig> endpointsConfig, AIModelValidator modelValidator)
        {
            _configuration = configuration;
            _endpointsConfig = endpointsConfig.Value;
            _modelValidator = modelValidator;
        }

        /// <summary>
        /// Calls Perplexity's API with the specified model and conversation history to generate a response.
        /// </summary>
        /// <param name="modelName">The name of the Perplexity model to use.</param>
        /// <param name="messages">
        /// A list of messages representing the conversation history, including user and assistant exchanges.
        /// </param>
        /// <param name="chatRequestSettings">
        /// Configuration options such as temperature and whether to remember chat history.
        /// </param>
        /// <returns>
        /// A <see cref="ProviderResult"/> containing the AI-generated response, token usage, calculated cost, and execution time.
        /// </returns>
        public async Task<ProviderResult> CallModelAsync(string modelName, List<Message> messages, ChatRequestSettings chatRequestSettings)
        {
            var stopwatch = new Stopwatch();

            try
            {
                // Validate supported models
                if (!_modelValidator.IsModelSupported("Perplexity", modelName))
                {
                    throw new ModelNotSupportedException($"Unsupported model: {modelName}");
                }

                // Get API key from configuration/environment
                string apiKey = _configuration["APIKeys:Perplexity"] ?? throw new KeyNotFoundException("Error: Perplexity API Key is missing");

                // Prepare the request body
                ProviderRequest requestBody = ChatRequestBuilder.BuildRequestBody(modelName, messages, chatRequestSettings);

                // Serialize request body to JSON
                var jsonRequest = JsonConvert.SerializeObject(requestBody);
                using var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Create and configure HttpClient
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                // Send POST request
                string providerBaseUrl = $"{_endpointsConfig.Providers["Perplexity"].BaseUrl}" ?? throw new KeyNotFoundException("Perplexity: Base URL is missing");
                string endpoint = $"{_endpointsConfig.Providers["Perplexity"].Endpoints["chat"]}" ?? throw new KeyNotFoundException("Perplexity: Endpoint config is missing");

                stopwatch = Stopwatch.StartNew();
                using var response = await httpClient.PostAsync($"{providerBaseUrl}{endpoint}", httpContent);
                stopwatch.Stop();

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Perplexity API request failed with status code {response.StatusCode}: {errorContent}");
                }

                // Read and deserialize the API response
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var completionResponse = JsonConvert.DeserializeObject<OpenAICompletionResponse>(jsonResponse) ?? new OpenAICompletionResponse();

                if (completionResponse == null || completionResponse.Choices == null || completionResponse.Choices.Count <= 0)
                {
                    throw new InvalidOperationException("Failed to parse a valid response from Perplexity API.");
                }

                // Extract token usage data (if available)
                int promptTokens = completionResponse.Usage?.PromptTokens ?? 0;
                int completionTokens = completionResponse.Usage?.CompletionTokens ?? 0;
                int totalTokens = completionResponse.Usage?.TotalTokens ?? (promptTokens + completionTokens);

                // Prepare the provider result using the first available choice
                var providerResult = new ProviderResult
                {
                    Message = completionResponse.Choices[0].Message.Content,
                    TotalTokens = totalTokens,
                    Cost = PricingService.CalculateCost("Perplexity", modelName, promptTokens, completionTokens),
                    TimeTaken = ElapsedTimeFormatter.FormatElapsedTime(stopwatch),
                };

                return providerResult;
            }
            finally
            {
                stopwatch.Stop();
            }
        }
    }
}
