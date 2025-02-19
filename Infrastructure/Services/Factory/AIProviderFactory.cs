// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Infrastructure.Services.Factory
{
    using System.Globalization;
    using ChatbotBenchmarkAPI.Infrastructure.Services.Interfaces;
    using ChatbotBenchmarkAPI.Infrastructure.Services.Providers;

    /// <summary>
    /// Factory responsible for creating instances of AI provider services based on provider name.
    /// </summary>
    public class AIProviderFactory : IAIProviderFactory
    {
        /// <summary>
        /// The service provider used to resolve dependencies.
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AIProviderFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider.</param>
        public AIProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the appropriate AI provider service implementation based on the provider name.
        /// </summary>
        /// <param name="providerName">The name of the AI provider to instantiate.</param>
        /// <returns>An implementation of IAIProviderService for the specified provider.</returns>
        /// <exception cref="NotSupportedException">Thrown when the specified provider is not supported.</exception>
        public IAIProviderService GetProviderService(string providerName)
        {
            return providerName?.ToLower(CultureInfo.InvariantCulture) switch
            {
                "openai" => _serviceProvider.GetRequiredService<OpenAIService>(),
                "deepseek" => _serviceProvider.GetRequiredService<DeepSeekService>(),
                "anthropic" => _serviceProvider.GetRequiredService<AnthropicService>(),
                "qwenai" => _serviceProvider.GetRequiredService<QwenAIService>(),
                "google" => _serviceProvider.GetRequiredService<GeminiService>(),
                "mistralai" => _serviceProvider.GetRequiredService<MistralAIService>(),
                "xai" => _serviceProvider.GetRequiredService<XAiService>(),
                _ => throw new NotSupportedException($"Provider '{providerName}' is not supported.")
            };
        }
    }
}
