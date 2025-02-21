// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Features.Chat
{
    using ChatbotBenchmarkAPI.Models.Request;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a chat request sent to an AI provider.
    /// </summary>
    public class ChatRequest
    {
        /// <summary>
        /// Gets or sets the AI provider name (e.g., OpenAI, DeepSeek).
        /// </summary>
        [JsonProperty("provider")]
        public string Provider { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the AI model to be used (e.g., GPT-4, DeepSeek R1).
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of messages exchanged in the conversation.
        /// </summary>
        [JsonProperty("messages")]
        public List<Message> Messages { get; set; } = new List<Message>();

        /// <summary>
        /// Gets or sets the configuration options such as temperature, whether to remember chat history and tokens limit.
        /// </summary>
        [JsonProperty("chatRequestSettings")]
        public ChatRequestSettings ChatRequestSettings { get; set; } = new ChatRequestSettings();
    }
}
