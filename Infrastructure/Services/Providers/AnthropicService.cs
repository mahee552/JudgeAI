// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Infrastructure.Services.Providers
{
    using ChatbotBenchmarkAPI.Business.Validation.ModelValidation;
    using ChatbotBenchmarkAPI.Infrastructure.Services.Interfaces;
    using ChatbotBenchmarkAPI.Models.Configurations.Endpoints;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Provides service implementation for interacting with the Anthropic AI provider.
    /// </summary>
    /// <remarks>
    /// This service handles communication with the Anthropic API, including authentication,
    /// request formatting, and response processing for AI-generated content.
    /// </remarks>
    /// <seealso cref="IAIProviderService"/>
    public class AnthropicService : BaseAIService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnthropicService"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="endpointsConfig">The AI endpoints configuration.</param>
        /// <param name="modelValidator">The validator for AI models.</param>
        public AnthropicService(IConfiguration configuration, IOptions<AIEndpointsConfig> endpointsConfig, AIModelValidator modelValidator)
        : base(configuration, endpointsConfig, modelValidator)
        {
        }

        /// <inheritdoc/>
        protected override string ProviderName => "Anthropic";

        /// <inheritdoc/>
        protected override string GetApiKey()
        {
            return Configuration["APIKeys:Anthropic"] ?? throw new KeyNotFoundException("Anthropic API Key is missing");
        }

        /// <inheritdoc/>
        protected override string GetBaseUrl()
        {
            return EndpointsConfig.Providers["Anthropic"].BaseUrl ?? throw new KeyNotFoundException("Anthropic Base URL is missing");
        }

        /// <inheritdoc/>
        protected override string GetChatEndpoint()
        {
            return EndpointsConfig.Providers["Anthropic"].Endpoints["chat"] ?? throw new KeyNotFoundException("Anthropic Chat endpoint is missing");
        }

        /// <inheritdoc/>
        protected override Dictionary<string, string> GetCustomHeaders()
        {
            return new Dictionary<string, string>
        {
            { "anthropic-version", "2023-06-01" },
        };
        }
    }
}
