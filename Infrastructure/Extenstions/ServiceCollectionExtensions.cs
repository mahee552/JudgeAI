namespace ChatbotBenchmarkAPI.Infrastructure.Extenstions
{
    using ChatbotBenchmarkAPI.Business.Validation.ModelValidation;
    using ChatbotBenchmarkAPI.Infrastructure.Services.Factories;
    using ChatbotBenchmarkAPI.Infrastructure.Services.Interfaces;
    using ChatbotBenchmarkAPI.Infrastructure.Services.Providers;

    /// <summary>
    /// Provides extension methods for registering AI-related services in the dependency injection container.
    /// </summary>
    /// <remarks>
    /// This class centralizes registrations for AI services (e.g., DeepSeek, Anthropic, Gemini) and their dependencies.
    /// </remarks>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all AI services with the DI container.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <returns>The service collection for method chaining.</returns>
        public static IServiceCollection AddAIServices(this IServiceCollection services)
        {
            services.AddTransient<OpenAIService>();
            services.AddTransient<DeepSeekService>();
            services.AddTransient<AnthropicService>();
            services.AddTransient<QwenAIService>();
            services.AddTransient<GeminiService>();
            services.AddTransient<MistralAIService>();
            services.AddTransient<XAiService>();
            services.AddTransient<PerplexityService>();
            services.AddTransient<AIModelValidator>();
            services.AddSingleton<IAIProviderFactory, AIProviderFactory>();

            return services;
        }
    }
}
