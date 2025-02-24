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
                if (!_modelValidator.IsModelSupported("Anthropic", modelName))
                {
                    throw new ModelNotSupportedException($"Unsupported model: {modelName}");
                }

                // Get API key from configuration/environment
                string apiKey = _configuration["APIKeys:Anthropic"] ?? throw new KeyNotFoundException("Error: Anthropic API Key is missing");

                // Prepare the request body
                ProviderRequest requestBody = ChatRequestBuilder.BuildRequestBody(modelName, messages, chatRequestSettings);

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
