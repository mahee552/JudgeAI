// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Models.CompletionResponses
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the complete response from a DeepSeek API completion request.
    /// </summary>
    public class DeepSeekCompletionResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier for the API response.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of the response object.
        /// </summary>
        [JsonProperty("object")]
        public string ObjectType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the timestamp of when the response was created (in Unix timestamp format).
        /// </summary>
        [JsonProperty("created")]
        public long Created { get; set; } = 0;

        /// <summary>
        /// Gets or sets the list of choices/completions returned by the API.
        /// </summary>
        [JsonProperty("choices")]
        public List<DeepSeekChoice> Choices { get; set; } = new List<DeepSeekChoice>();

        /// <summary>
        /// Gets or sets the token usage statistics for the API request.
        /// </summary>
        [JsonProperty("usage")]
        public DeepSeekTokenUsage Usage { get; set; } = new DeepSeekTokenUsage();
    }

    /// <summary>
    /// Represents a single choice/completion in the DeepSeek API response.
    /// </summary>
    public class DeepSeekChoice
    {
        /// <summary>
        /// Gets or sets the zero-based index of this choice in the response.
        /// </summary>
        [JsonProperty("index")]
        public int Index { get; set; } = 0;

        /// <summary>
        /// Gets or sets the message content for this choice.
        /// </summary>
        [JsonProperty("message")]
        public DeepSeekMessage Message { get; set; } = new DeepSeekMessage();

        /// <summary>
        /// Gets or sets the reason for finishing the generation (e.g., 'stop', 'length').
        /// </summary>
        [JsonProperty("finish_reason")]
        public string FinishReason { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a message in the DeepSeek API response.
    /// </summary>
    public class DeepSeekMessage
    {
        /// <summary>
        /// Gets or sets the role of the message (e.g., 'system', 'user', 'assistant').
        /// </summary>
        [JsonProperty("role")]
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the textual content of the message.
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents token usage statistics for a DeepSeek API request.
    /// </summary>
    public class DeepSeekTokenUsage
    {
        /// <summary>
        /// Gets or sets the number of tokens in the input prompt.
        /// </summary>
        [JsonProperty("prompt_tokens")]
        public int PromptTokens { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of tokens in the generated completion.
        /// </summary>
        [JsonProperty("completion_tokens")]
        public int CompletionTokens { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total number of tokens used in the request.
        /// </summary>
        [JsonProperty("total_tokens")]
        public int TotalTokens { get; set; } = 0;
    }
}
