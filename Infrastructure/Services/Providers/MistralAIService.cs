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
    /// Service implementation for interacting with Mistral AI's API endpoints.
    /// Handles model calls, token counting, and cost calculations specific to Mistral AI models.
    /// </summary>
    public class MistralAIService : BaseAIService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MistralAIService"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="endpointsConfig">The AI endpoints configuration.</param>
        /// <param name="modelValidator">The validator for AI models.</param>
        public MistralAIService(IConfiguration configuration, IOptions<AIEndpointsConfig> endpointsConfig, AIModelValidator modelValidator)
        : base(configuration, endpointsConfig, modelValidator)
        {
        }

        /// <inheritdoc/>
        protected override string ProviderName => "MistralAI";

        /// <inheritdoc/>
        protected override string GetApiKey()
        {
            return Configuration["APIKeys:MistralAI"] ?? throw new KeyNotFoundException("MistralAI API Key is missing");
        }

        /// <inheritdoc/>
        protected override string GetBaseUrl()
        {
            return EndpointsConfig.Providers["MistralAI"].BaseUrl ?? throw new KeyNotFoundException("MistralAI Base URL is missing");
        }

        /// <inheritdoc/>
        protected override string GetChatEndpoint()
        {
            return EndpointsConfig.Providers["MistralAI"].Endpoints["chat"] ?? throw new KeyNotFoundException("MistralAI Chat endpoint is missing");
        }
    }
}
