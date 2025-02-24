// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Models.Request
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a request for an AI model call.
    /// </summary>
    public class ProviderRequest
    {
        /// <summary>
        /// Gets or sets the AI model name.
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the maximum number of tokens for the response.
        /// </summary>
        [JsonProperty("max_tokens")]
        public int MaxTokens { get; set; }

        /// <summary>
        /// Gets or sets the conversation history messages.
        /// </summary>
        [JsonProperty("messages")]
        public List<Message> Messages { get; set; } = new List<Message>();

        /// <summary>
        /// Gets or sets the temperature setting for response randomness.
        /// </summary>
        [JsonProperty("temperature")]
        public double Temperature { get; set; }
    }
}
