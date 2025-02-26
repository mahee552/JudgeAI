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
    /// Abstract base class that implements the IAIProviderService interface.
    /// Provides common functionality for AI provider services while requiring
    /// derived classes to implement specific behaviors.
    /// </summary>
    public abstract class BaseAIService : IAIProviderService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAIService"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="endpointsConfig">The AI endpoints configuration.</param>
        /// <param name="modelValidator">The validator for AI models.</param>
        protected BaseAIService(IConfiguration configuration, IOptions<AIEndpointsConfig> endpointsConfig, AIModelValidator modelValidator)
        {
            Configuration = configuration;
            EndpointsConfig = endpointsConfig.Value;
            ModelValidator = modelValidator;
        }

        /// <summary>
        /// Gets Configuration used to access application settings.
        /// </summary>
        protected IConfiguration Configuration { get; }

        /// <summary>
        /// Gets Configuration containing AI endpoint information.
        /// </summary>
        protected AIEndpointsConfig EndpointsConfig { get; }

        /// <summary>
        /// Gets Validator for AI models.
        /// </summary>
        protected AIModelValidator ModelValidator { get; }

        /// <summary>
        /// Gets Stopwatch used for performance monitoring and timing operations.
        /// </summary>
        protected Stopwatch Stopwatch { get; } = new();

        /// <summary>
        /// Gets the name of the AI provider.
        /// </summary>
        protected abstract string ProviderName { get; }

        /// <inheritdoc/>
        public async Task<ProviderResult> CallModelAsync(string modelName, List<Message> messages, ChatRequestSettings chatRequestSettings)
        {
            try
            {
                using var response = await SendRequestAsync(modelName, messages, chatRequestSettings);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"{ProviderName} API request failed: {errorContent}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var completionResponse = JsonConvert.DeserializeObject<OpenAICompletionResponse>(jsonResponse) ?? new();

                if (completionResponse.Choices == null || completionResponse.Choices.Count == 0)
                {
                    throw new InvalidOperationException("Failed to parse a valid response from API.");
                }

                int promptTokens = completionResponse.Usage?.PromptTokens ?? 0;
                int completionTokens = completionResponse.Usage?.CompletionTokens ?? 0;
                int totalTokens = completionResponse.Usage?.TotalTokens ?? (promptTokens + completionTokens);

                return new ProviderResult
                {
                    Message = completionResponse.Choices[0].Message.Content,
                    TotalTokens = totalTokens,
                    Cost = PricingService.CalculateCost(ProviderName, modelName, promptTokens, completionTokens),
                    TimeTaken = ElapsedTimeFormatter.FormatElapsedTime(Stopwatch),
                };
            }
            finally
            {
                Stopwatch.Stop();
            }
        }

        /// <inheritdoc/>
        public async Task StreamModelResponseAsync(string modelName, List<Message> messages, ChatRequestSettings chatRequestSettings, HttpResponse response)
        {
            try
            {
                using var httpResponse = await SendRequestAsync(modelName, messages, chatRequestSettings, HttpCompletionOption.ResponseHeadersRead);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    var errorContent = await httpResponse.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"{ProviderName} API request failed: {errorContent}");
                }

                response.ContentType = "text/event-stream";
                response.Headers.CacheControl = "no-cache";
                response.Headers.Connection = "keep-alive";

                await using var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(responseStream);

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    if (line.StartsWith("data: ", StringComparison.OrdinalIgnoreCase))
                    {
                        var jsonData = line.Substring(6).Trim();

                        if (jsonData == "[DONE]")
                        {
                            await response.WriteAsync("data: [DONE]\n");
                            break;
                        }

                        var parsedData = JsonConvert.DeserializeObject<dynamic>(jsonData);
                        if (parsedData?.choices != null && parsedData?.choices.Count > 0)
                        {
                            var delta = parsedData?.choices[0].delta;
                            if (delta?.content != null)
                            {
                                var formattedResponse = JsonConvert.SerializeObject(new { v = Convert.ToString(delta.content) });

                                await response.WriteAsync($"data: {formattedResponse}\n\n");
                                await response.Body.FlushAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var errorResponse = JsonConvert.SerializeObject(new { error = ex.Message });
                await response.WriteAsync($"data: {errorResponse}\n\n");
                await response.Body.FlushAsync();
                throw;
            }
        }

        /// <summary>
        /// Retrieves the API key used for authentication with the AI provider.
        /// </summary>
        /// <returns>The API key as a string.</returns>
        protected abstract string GetApiKey();

        /// <summary>
        /// Retrieves the base URL for the AI provider's API.
        /// </summary>
        /// <returns>The base URL as a string.</returns>
        protected abstract string GetBaseUrl();

        /// <summary>
        /// Retrieves the endpoint path for chat completions.
        /// </summary>
        /// <returns>The chat endpoint path as a string.</returns>
        protected abstract string GetChatEndpoint();

        /// <summary>
        /// Gets custom HTTP headers to be included with API requests.
        /// </summary>
        /// <returns>A dictionary containing custom header names and values.</returns>
        protected virtual Dictionary<string, string> GetCustomHeaders()
        {
            return new Dictionary<string, string>(); // Default: No extra headers
        }

        private async Task<HttpResponseMessage> SendRequestAsync(
            string modelName,
            List<Message> messages,
            ChatRequestSettings chatRequestSettings,
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            if (!ModelValidator.IsModelSupported(ProviderName, modelName))
            {
                throw new ModelNotSupportedException($"Unsupported model: {modelName}");
            }

            string apiKey = GetApiKey();
            ProviderRequest requestBody = ChatRequestBuilder.BuildRequestBody(modelName, messages, chatRequestSettings);
            var jsonRequest = JsonConvert.SerializeObject(requestBody);
            using var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var customHeaders = GetCustomHeaders();

            // Apply custom headers, if any.
            foreach (var header in customHeaders)
            {
                if (!httpClient.DefaultRequestHeaders.Contains(header.Key))
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            string endpoint = $"{GetBaseUrl()}{GetChatEndpoint()}";

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = httpContent };

            Stopwatch.Start();
            var response = await httpClient.SendAsync(requestMessage, completionOption);
            Stopwatch.Stop();

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"{ProviderName} API request failed: {errorContent}");
            }

            return response;
        }
    }
}
