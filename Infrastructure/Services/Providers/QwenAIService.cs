// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Infrastructure.Services.Providers
{
    using ChatbotBenchmarkAPI.Business.Validation.ModelValidation;
    using ChatbotBenchmarkAPI.Models.Configurations.Endpoints;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Service implementation for interacting with QwenAI's API endpoints.
    /// Handles model calls, token counting, and cost calculations specific to QwenAI models.
    /// </summary>
    public class QwenAIService : BaseAIService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QwenAIService"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="endpointsConfig">The AI endpoints configuration.</param>
        /// <param name="modelValidator">The validator for AI models.</param>
        public QwenAIService(IConfiguration configuration, IOptions<AIEndpointsConfig> endpointsConfig, AIModelValidator modelValidator)
        : base(configuration, endpointsConfig, modelValidator)
        {
        }

        /// <inheritdoc/>
        protected override string ProviderName => "QwenAI";

        /// <inheritdoc/>
        protected override string GetApiKey()
        {
            return Configuration["APIKeys:QwenAI"] ?? throw new KeyNotFoundException("QwenAI API Key is missing");
        }

        /// <inheritdoc/>
        protected override string GetBaseUrl()
        {
            return EndpointsConfig.Providers["QwenAI"].BaseUrl ?? throw new KeyNotFoundException("QwenAI Base URL is missing");
        }

        /// <inheritdoc/>
        protected override string GetChatEndpoint()
        {
            return EndpointsConfig.Providers["QwenAI"].Endpoints["chat"] ?? throw new KeyNotFoundException("QwenAI Chat endpoint is missing");
        }
    }
}
