// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Infrastructure.Services.Providers
{
    using System.Diagnostics;
    using System.Text;
    using ChatbotBenchmarkAPI.Business.Pricing;
    using ChatbotBenchmarkAPI.Business.Validation.ModelValidation;
    using ChatbotBenchmarkAPI.Exceptions;
    using ChatbotBenchmarkAPI.Models.CompletionResponses;
    using ChatbotBenchmarkAPI.Models.Configurations.Endpoints;
    using ChatbotBenchmarkAPI.Models.Request;
    using ChatbotBenchmarkAPI.Models.Request.Gemini;
    using ChatbotBenchmarkAPI.Models.Response;
    using ChatbotBenchmarkAPI.Utilities.Builders;
    using ChatbotBenchmarkAPI.Utilities.Formatters;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// Service implementation for interacting with Google's Gemini API.
    /// Handles model calls, token counting, and cost calculations.
    /// </summary>
    public class GeminiService : BaseAIService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeminiService"/> class.
        /// </summary>
        /// <param name="configuration">IConfiguration.</param>
        /// <param name="endpointsConfig">AIEndpointsConfig.</param>
        /// <param name="modelValidator">The model validation service.</param>
        public GeminiService(
            IConfiguration configuration,
            IOptions<AIEndpointsConfig> endpointsConfig,
            AIModelValidator modelValidator)
            : base(configuration, endpointsConfig, modelValidator)
        {
        }

        /// <inheritdoc/>
        protected override string ProviderName => "Google";

        /// <summary>
        /// Calls Google's Gemini API with the specified model and prompt to generate a response.
        /// </summary>
        /// <param name="modelName">The name of the Anthropic model to use (e.g., "gemini-pro", "claude-3-sonnet-20240229").</param>
        /// <param name="messages">
        /// A list of messages representing the conversation history, including user and assistant exchanges.
        /// </param>
        /// <param name="chatRequestSettings">
        /// Configuration options such as temperature and whether to remember chat history.
        /// </param>
        /// <returns>
        /// A <see cref="ProviderResult"/> containing the AI-generated response, token usage, calculated cost, and execution time.
        /// </returns>
        public override async Task<ProviderResult> CallModelAsync(string modelName, List<Message> messages, ChatRequestSettings chatRequestSettings)
        {
            var stopwatch = new Stopwatch();

            try
            {
                // Validate supported models
                if (!ModelValidator.IsModelSupported(ProviderName, modelName))
                {
                    throw new ModelNotSupportedException($"Unsupported model: {modelName}");
                }

                // Get API key from configuration
                string apiKey = GetApiKey();

                // Build Gemini API request
                GeminiRequest requestBody = ChatRequestBuilder.BuildGeminiRequestBody(messages, chatRequestSettings);

                // Serialize request
                string jsonRequest = JsonConvert.SerializeObject(requestBody);
                using var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Configure HTTP client
                using var httpClient = new HttpClient();

                // Send request to Gemini API
                string requestUrl = $"{GetBaseUrl()}{GetChatEndpoint()}{modelName}:generateContent?key={apiKey}";

                stopwatch = Stopwatch.StartNew();
                using var response = await httpClient.PostAsync(requestUrl, httpContent);
                stopwatch.Stop();

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

                // Calculate tokens and cost
                int promptTokens = completionResponse.UsageMetadata?.PromptTokenCount ?? 0;
                int completionTokens = completionResponse.UsageMetadata?.CandidatesTokenCount ?? 0;
                int totalTokens = completionResponse.UsageMetadata?.TotalTokenCount
                    ?? (promptTokens + completionTokens);

                return new ProviderResult
                {
                    Message = completionResponse.Candidates[0].Content.Parts[0].Text,
                    TotalTokens = totalTokens,
                    Cost = PricingService.CalculateCost(ProviderName, modelName, promptTokens, completionTokens),
                    TimeTaken = ElapsedTimeFormatter.FormatElapsedTime(stopwatch),
                };
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        /// <inheritdoc/>
        public override async Task StreamModelResponseAsync(string modelName, List<Message> messages, ChatRequestSettings chatRequestSettings, HttpResponse response)
        {
            var stopwatch = new Stopwatch();

            try
            {
                // Validate supported models
                if (!ModelValidator.IsModelSupported(ProviderName, modelName))
                {
                    throw new ModelNotSupportedException($"Unsupported model: {modelName}");
                }

                // Get API key from configuration
                string apiKey = GetApiKey();

                // Build Gemini API request
                GeminiRequest requestBody = ChatRequestBuilder.BuildGeminiRequestBody(messages, chatRequestSettings);

                // Serialize request
                string jsonRequest = JsonConvert.SerializeObject(requestBody);
                using var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Configure HTTP client
                using var httpClient = new HttpClient();

                // Send request to Gemini API with stream=true
                string requestUrl = $"{GetBaseUrl()}{GetChatEndpoint()}{modelName}:streamGenerateContent?key={apiKey}";

                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl) { Content = httpContent };

                stopwatch.Start();
                var httpResponse = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
                stopwatch.Stop();

                if (!httpResponse.IsSuccessStatusCode)
                {
                    string errorContent = await httpResponse.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Gemini API request failed with status code {httpResponse.StatusCode}: {errorContent}");
                }

                response.ContentType = "text/event-stream";
                response.Headers.CacheControl = "no-cache";
                response.Headers.Connection = "keep-alive";

                await using var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(responseStream);

                while (!reader.EndOfStream)
                {
                    var chunk = await reader.ReadLineAsync();

                    if (string.IsNullOrWhiteSpace(chunk))
                    {
                        continue;
                    }

                    await response.WriteAsync($"data: {chunk}\n\n");
                    await response.Body.FlushAsync();
                }
            }
            catch (Exception ex)
            {
                var errorResponse = JsonConvert.SerializeObject(new { error = ex.Message });
                await response.WriteAsync($"data: {errorResponse}\n\n");
                await response.Body.FlushAsync();
                throw;
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        /// <inheritdoc/>
        protected override string GetApiKey()
        {
            return Configuration["APIKeys:Google"] ?? throw new KeyNotFoundException("Google API Key is missing");
        }

        /// <inheritdoc/>
        protected override string GetBaseUrl()
        {
            return EndpointsConfig.Providers["Google"].BaseUrl ?? throw new KeyNotFoundException("Google Base URL is missing");
        }

        /// <inheritdoc/>
        protected override string GetChatEndpoint()
        {
            return EndpointsConfig.Providers["Google"].Endpoints["chat"] ?? throw new KeyNotFoundException("Google Chat endpoint is missing");
        }
    }
}
