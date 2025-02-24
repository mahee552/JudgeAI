// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Utilities.Builders
{
    using ChatbotBenchmarkAPI.Models.Request;

    /// <summary>
    /// Responsible for building the request body for AI model API calls.
    /// </summary>
    public static class ChatRequestBuilder
    {
        /// <summary>
        /// Builds the request body for an AI model call based on chat settings.
        /// </summary>
        /// <param name="modelName">The AI model name (e.g., "gpt-4").</param>
        /// <param name="messages">The conversation history messages.</param>
        /// <param name="chatRequestSettings">The chat request settings (temperature, max tokens, etc.).</param>
        /// <returns>An object representing the request body.</returns>
        public static ProviderRequest BuildRequestBody(string modelName, List<Message> messages, ChatRequestSettings chatRequestSettings)
        {
            if (!chatRequestSettings.RememberHistory)
            {
                Message message = new Message
                {
                    Role = "user",
                    Content = messages.LastOrDefault()?.Content ?? string.Empty,
                };

                return new ProviderRequest
                {
                    Model = modelName,
                    MaxTokens = chatRequestSettings.MaxTokens,
                    Messages = new List<Message> { message },
                    Temperature = chatRequestSettings.Temperature,
                };
            }

            return new ProviderRequest
            {
                Model = modelName,
                MaxTokens = chatRequestSettings.MaxTokens,
                Messages = messages,
                Temperature = chatRequestSettings.Temperature,
            };
        }
    }
}
