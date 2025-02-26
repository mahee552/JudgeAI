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
    /// Service implementation for interacting with X.AI's Grok API endpoints.
    /// Handles model calls, token counting, and cost calculations specific to Grok models.
    /// </summary>
    public class XAiService : BaseAIService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XAiService"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="endpointsConfig">The AI endpoints configuration.</param>
        /// <param name="modelValidator">The validator for AI models.</param>
        public XAiService(IConfiguration configuration, IOptions<AIEndpointsConfig> endpointsConfig, AIModelValidator modelValidator)
        : base(configuration, endpointsConfig, modelValidator)
        {
        }

        /// <inheritdoc/>
        protected override string ProviderName => "XAI";

        /// <inheritdoc/>
        protected override string GetApiKey()
        {
            return Configuration["APIKeys:XAI"] ?? throw new KeyNotFoundException("XAI API Key is missing");
        }

        /// <inheritdoc/>
        protected override string GetBaseUrl()
        {
            return EndpointsConfig.Providers["XAI"].BaseUrl ?? throw new KeyNotFoundException("XAI Base URL is missing");
        }

        /// <inheritdoc/>
        protected override string GetChatEndpoint()
        {
            return EndpointsConfig.Providers["XAI"].Endpoints["chat"] ?? throw new KeyNotFoundException("XAI Chat endpoint is missing");
        }
    }
}
