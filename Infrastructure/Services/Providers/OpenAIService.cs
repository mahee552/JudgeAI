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
    /// Service implementation for interacting with OpenAI's API endpoints.
    /// Handles model calls, token counting, and cost calculations specific to OpenAI models.
    /// </summary>
    public class OpenAIService : BaseAIService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAIService"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="endpointsConfig">The AI endpoints configuration.</param>
        /// <param name="modelValidator">The validator for AI models.</param>
        public OpenAIService(IConfiguration configuration, IOptions<AIEndpointsConfig> endpointsConfig, AIModelValidator modelValidator)
        : base(configuration, endpointsConfig, modelValidator)
        {
        }

        /// <inheritdoc/>
        protected override string ProviderName => "OpenAI";

        /// <inheritdoc/>
        protected override string GetApiKey()
        {
            return Configuration["APIKeys:OpenAI"] ?? throw new KeyNotFoundException("OpenAI API Key is missing");
        }

        /// <inheritdoc/>
        protected override string GetBaseUrl()
        {
            return EndpointsConfig.Providers["OpenAI"].BaseUrl ?? throw new KeyNotFoundException("OpenAI Base URL is missing");
        }

        /// <inheritdoc/>
        protected override string GetChatEndpoint()
        {
            return EndpointsConfig.Providers["OpenAI"].Endpoints["chat"] ?? throw new KeyNotFoundException("OpenAI Chat endpoint is missing");
        }
    }
}
