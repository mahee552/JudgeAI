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
        [JsonProperty("max_tokens", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int MaxTokens { get; set; }

        /// <summary>
        /// Gets or sets the conversation history messages.
        /// </summary>
        [JsonProperty("messages")]
        public List<Message> Messages { get; set; } = new List<Message>();

        /// <summary>
        /// Gets or sets the temperature setting for response randomness.
        /// </summary>
        [JsonProperty("temperature", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double Temperature { get; set; }

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
        [JsonProperty("stream")]
        public bool Stream { get; set; } = false;
    }
}
