// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Models.CompletionResponses
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the response from the OpenAI completion API.
    /// </summary>
    public class MistralAICompletionResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier for the response.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of object returned.
        /// </summary>
        [JsonProperty("object")]
        public string ObjectType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the timestamp when the response was created.
        /// </summary>
        [JsonProperty("created")]
        public int Created { get; set; }

        /// <summary>
        /// Gets or sets the model used for the completion.
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the usage information for the request.
        /// </summary>
        [JsonProperty("usage")]
        public Usage Usage { get; set; } = new Usage();

        /// <summary>
        /// Gets or sets the list of choices returned by the model.
        /// </summary>
        [JsonProperty("choices")]
        public List<Choice> Choices { get; set; } = new List<Choice>();
    }
}