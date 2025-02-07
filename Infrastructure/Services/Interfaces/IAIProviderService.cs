// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Infrastructure.Services.Interfaces
{
    using ChatbotBenchmarkAPI.Features.Compare;

    /// <summary>
    /// Defines the interface for services that interact with external AI provider APIs.
    /// </summary>
    public interface IAIProviderService
    {
        /// <summary>
        /// Calls the external AI model API.
        /// </summary>
        /// <param name="modelName">The name of the model to use (e.g., "gpt-4").</param>
        /// <param name="prompt">The input prompt.</param>
        /// <returns>A ProviderResult containing the response, token count, cost, and time taken.</returns>
        Task<ProviderResult> CallModelAsync(string modelName, string prompt);
    }
}
