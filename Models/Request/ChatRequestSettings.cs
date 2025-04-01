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
        [JsonProperty("temperature", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double Temperature { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to remember chat history.
        /// </summary>
        /// <remarks>
        /// If set to <c>true</c>, previous messages are considered in the response.
        /// If <c>false</c>, each request is processed independently.
        /// </remarks>
        [JsonProperty("rememberHistory", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool RememberHistory { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of tokens allowed in the model's response.
        /// </summary>
        /// <remarks>
        /// Tokens include words, punctuation, and spaces. Setting a higher value allows longer responses,
        /// but may increase cost and processing time.
        /// </remarks>
        [JsonProperty("max_tokens", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int MaxTokens { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the response should be streamed.
        /// </summary>
        /// <remarks>
        /// When set to <c>true</c>, the API will send the response in chunks as the data is generated,
        /// allowing for real-time processing and display. This is particularly useful for applications
        /// requiring low latency, such as chatbots or interactive text generation.
        /// When set to <c>false</c>, the API will wait until the entire response is generated before
        /// sending it back to the client.
        /// </remarks>
        /// <example>
        /// <code>
        /// var request = new ApiRequest
        /// {
        ///     Stream = true // Enable streaming for real-time responses
        /// };
        /// </code>
        /// </example>
        [JsonProperty("stream", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Stream { get; set; } = false;
    }
}
