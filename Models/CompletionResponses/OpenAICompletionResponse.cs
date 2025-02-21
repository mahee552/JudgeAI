// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Models.CompletionResponses
{
    using ChatbotBenchmarkAPI.Models.Request;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the response from the OpenAI completion API.
    /// </summary>
    public class OpenAICompletionResponse
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

    /// <summary>
    /// Represents the usage statistics for the OpenAI completion request.
    /// </summary>
    public class Usage
    {
        /// <summary>
        /// Gets or sets the number of tokens used in the prompt.
        /// </summary>
        [JsonProperty("prompt_tokens")]
        public int PromptTokens { get; set; }

        /// <summary>
        /// Gets or sets the number of tokens used in the completion.
        /// </summary>
        [JsonProperty("completion_tokens")]
        public int CompletionTokens { get; set; }

        /// <summary>
        /// Gets or sets the total number of tokens used in the request.
        /// </summary>
        [JsonProperty("total_tokens")]
        public int TotalTokens { get; set; }
    }

    /// <summary>
    /// Represents a choice returned by the OpenAI completion API.
    /// </summary>
    public class Choice
    {
        /// <summary>
        /// Gets or sets the message associated with the choice.
        /// </summary>
        [JsonProperty("message")]
        public Message Message { get; set; } = new Message();

        /// <summary>
        /// Gets or sets the reason for finishing the response.
        /// </summary>
        [JsonProperty("finish_reason")]
        public string FinishReason { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the index of the choice in the list of choices.
        /// </summary>
        [JsonProperty("index")]
        public int Index { get; set; }
    }
}
