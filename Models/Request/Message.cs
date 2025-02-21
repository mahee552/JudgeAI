// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Models.Request
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a single message in a chat conversation.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Gets or sets the role of the message sender (e.g., "user", "assistant", "system").
        /// </summary>
        [JsonProperty("role")]
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; } = string.Empty;
    }
}
