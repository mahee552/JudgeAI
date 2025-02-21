// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Infrastructure.Services.Interfaces
{
    using ChatbotBenchmarkAPI.Models.Request;
    using ChatbotBenchmarkAPI.Models.Response;

    /// <summary>
    /// Defines the interface for services that interact with external AI provider APIs.
    /// </summary>
    public interface IAIProviderService
    {
        /// <summary>
        /// Calls the external AI model API and processes the response.
        /// </summary>
        /// <param name="modelName">The name of the AI model to use (e.g., "gpt-4").</param>
        /// <param name="messages">The conversation history, including user and assistant messages.</param>
        /// <param name="chatRequestSettings">Configuration options such as temperature and history retention.</param>
        /// <returns>
        /// A <see cref="ProviderResult"/> containing the AI-generated response, token count, cost, and execution time.
        /// </returns>
        Task<ProviderResult> CallModelAsync(string modelName, List<Message> messages, ChatRequestSettings chatRequestSettings);
    }
}
