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
    /// Service implementation for interacting with Perplexity's API endpoints.
    /// Handles model calls, token counting, and cost calculations specific to Perplexity models.
    /// </summary>
    public class PerplexityService : BaseAIService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PerplexityService"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="endpointsConfig">The AI endpoints configuration.</param>
        /// <param name="modelValidator">The validator for AI models.</param>
        public PerplexityService(IConfiguration configuration, IOptions<AIEndpointsConfig> endpointsConfig, AIModelValidator modelValidator)
        : base(configuration, endpointsConfig, modelValidator)
        {
        }

        /// <inheritdoc/>
        protected override string ProviderName => "Perplexity";

        /// <inheritdoc/>
        protected override string GetApiKey()
        {
            return Configuration["APIKeys:Perplexity"] ?? throw new KeyNotFoundException("Perplexity API Key is missing");
        }

        /// <inheritdoc/>
        protected override string GetBaseUrl()
        {
            return EndpointsConfig.Providers["Perplexity"].BaseUrl ?? throw new KeyNotFoundException("Perplexity Base URL is missing");
        }

        /// <inheritdoc/>
        protected override string GetChatEndpoint()
        {
            return EndpointsConfig.Providers["Perplexity"].Endpoints["chat"] ?? throw new KeyNotFoundException("Perplexity Chat endpoint is missing");
        }
    }
}
