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
    /// Provides service implementation for interacting with the DeepSeek AI provider.
    /// </summary>
    /// <remarks>
    /// This service handles communication with the DeepSeek API, including authentication,
    /// request formatting, and response processing for AI-generated content.
    /// </remarks>
    /// <seealso cref="BaseAIService"/>
    public class DeepSeekService : BaseAIService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeepSeekService"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="endpointsConfig">The AI endpoints configuration.</param>
        /// <param name="modelValidator">The validator for AI models.</param>
        public DeepSeekService(IConfiguration configuration, IOptions<AIEndpointsConfig> endpointsConfig, AIModelValidator modelValidator)
        : base(configuration, endpointsConfig, modelValidator)
        {
        }

        /// <inheritdoc/>
        protected override string ProviderName => "DeepSeek";

        /// <inheritdoc/>
        protected override string GetApiKey()
        {
            return Configuration["APIKeys:DeepSeek"] ?? throw new KeyNotFoundException("DeepSeek API Key is missing");
        }

        /// <inheritdoc/>
        protected override string GetBaseUrl()
        {
            return EndpointsConfig.Providers["DeepSeek"].BaseUrl ?? throw new KeyNotFoundException("DeepSeek Base URL is missing");
        }

        /// <inheritdoc/>
        protected override string GetChatEndpoint()
        {
            return EndpointsConfig.Providers["DeepSeek"].Endpoints["chat"] ?? throw new KeyNotFoundException("DeepSeek Chat endpoint is missing");
        }
    }
}