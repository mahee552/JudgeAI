// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// Defines the interface for factory services that create AI provider instances.
    /// </summary>
    public interface IAIProviderFactory
    {
        /// <summary>
        /// Gets the appropriate AI provider service implementation based on the provider name.
        /// </summary>
        /// <param name="providerName">The name of the AI provider to instantiate.</param>
        /// <returns>An implementation of IAIProviderService for the specified provider.</returns>
        IAIProviderService GetProviderService(string providerName);
    }
}
