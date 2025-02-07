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
    /// Service implementation for interacting with OpenAI's API endpoints.
    /// Handles model calls, token counting, and cost calculations specific to OpenAI models.
    /// </summary>
    public class OpenAIService : IAIProviderService
    {
        private readonly IConfiguration _configuration;
        private readonly AIEndpointsConfig _endpointsConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAIService"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration.</param>
        /// <param name="endpointsConfig">AIEndpointsConfig.</param>
        public OpenAIService(IConfiguration configuration, IOptions<AIEndpointsConfig> endpointsConfig)
        {
            _configuration = configuration;
            _endpointsConfig = endpointsConfig.Value;
        }

        /// <summary>
        /// Calls OpenAI's API with the specified model and prompt to generate a response.
        /// </summary>
        /// <param name="modelName">The name of the OpenAI model to use (e.g., "gpt-4", "gpt-3.5-turbo").</param>
        /// <param name="prompt">The input prompt to send to the model.</param>
        /// <returns>A ProviderResult containing the model's response, token usage, calculated cost, and time taken.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
        /// <exception cref="ArgumentException">Thrown when the model name is invalid or unsupported.</exception>
        public async Task<ProviderResult> CallModelAsync(string modelName, string prompt)
        {
            // Validate supported models
            if (modelName is not "gpt-3.5-turbo" and not "gpt-4")
            {
                throw new ArgumentException($"Unsupported model: {modelName}");
            }

            // Get API key from configuration/environment
            string apiKey = _configuration["APIKey"] ?? string.Empty;

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
            };

            // Serialize request body to JSON
            var jsonRequest = JsonConvert.SerializeObject(requestBody);
            using var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // Create and configure HttpClient
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            // Send POST request
            string providerbaseUrl = $"{_endpointsConfig.Providers["OpenAI"].BaseUrl}";
            string endpoint = $"{_endpointsConfig.Providers["OpenAI"].Endpoints["chat"]}";

            using var response = await httpClient.PostAsync($"{providerbaseUrl}{endpoint}", httpContent);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"OpenAI API request failed with status code {response.StatusCode}: {errorContent}");
            }

            // Read and deserialize the API response
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var completionResponse = JsonConvert.DeserializeObject<OpenAICompletionResponse>(jsonResponse) ?? new OpenAICompletionResponse();
            stopwatch.Stop();

            if (completionResponse == null || completionResponse.Choices == null || completionResponse.Choices.Count <= 0)
            {
                throw new InvalidOperationException("Failed to parse a valid response from OpenAI API.");
            }

            // Extract token usage data (if available)
            int promptTokens = completionResponse.Usage?.PromptTokens ?? 0;
            int completionTokens = completionResponse.Usage?.CompletionTokens ?? 0;
            int totalTokens = completionResponse.Usage?.TotalTokens ?? (promptTokens + completionTokens);

            // Calculate cost based on dynamic pricing rules
            decimal cost = 0m;
            if (modelName == "gpt-3.5-turbo")
            {
                // GPT-3.5-turbo: $0.002 per 1K tokens
                cost = totalTokens / 1000m * 0.002m;
            }
            else if (modelName == "gpt-4")
            {
                // GPT-4: $0.03 per 1K prompt tokens and $0.06 per 1K completion tokens
                cost = (promptTokens / 1000m * 0.03m) + (completionTokens / 1000m * 0.06m);
            }

            // Prepare the provider result using the first available choice
            var providerResult = new ProviderResult
            {
                Message = completionResponse.Choices[0].Message.Content,
                TotalTokens = totalTokens,
                Cost = cost,
                TimeTakenMs = stopwatch.ElapsedMilliseconds,
            };

            return providerResult;
        }
    }
}
