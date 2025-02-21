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
        public static object BuildRequestBody(string modelName, List<Message> messages, ChatRequestSettings chatRequestSettings)
        {
            if (!chatRequestSettings.RememberHistory)
            {
                Message message = new Message
                {
                    Role = "user",
                    Content = messages.LastOrDefault()?.Content ?? string.Empty,
                };

                return new
                {
                    model = modelName,
                    max_tokens = chatRequestSettings.MaxTokens,
                    messages = new List<Message> { message },
                    temperature = chatRequestSettings.Temperature,
                };
            }

            return new
            {
                model = modelName,
                max_tokens = chatRequestSettings.MaxTokens,
                messages,
                temperature = chatRequestSettings.Temperature,
            };
        }
    }
}
