// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Models.Request
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents configuration settings for a chat request.
    /// </summary>
    public class ChatRequestSettings
    {
        /// <summary>
        /// Gets or sets the temperature value for AI responses.
        /// </summary>
        /// <remarks>
        /// A higher value (e.g., 1.0) makes responses more random,
        /// while a lower value (e.g., 0.2) makes them more deterministic.
        /// </remarks>
        [JsonProperty("temperature")]
        public double Temperature { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to remember chat history.
        /// </summary>
        /// <remarks>
        /// If set to <c>true</c>, previous messages are considered in the response.
        /// If <c>false</c>, each request is processed independently.
        /// </remarks>
        [JsonProperty("rememberHistory")]
        public bool RememberHistory { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of tokens allowed in the model's response.
        /// </summary>
        /// <remarks>
        /// Tokens include words, punctuation, and spaces. Setting a higher value allows longer responses,
        /// but may increase cost and processing time.
        /// </remarks>
        [JsonProperty("max_tokens")]
        public int MaxTokens { get; set; }
    }
}
