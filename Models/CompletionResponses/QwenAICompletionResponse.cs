// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Models.CompletionResponses
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the completion response from the QwenAI API.
    /// </summary>
    public class QwenAICompletionResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier for the completion response.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of completion choices returned by the API.
        /// </summary>
        [JsonProperty("choices")]
        public List<QwenAIChoice> Choices { get; set; } = new List<QwenAIChoice>();

        /// <summary>
        /// Gets or sets the token usage information for this completion.
        /// </summary>
        [JsonProperty("usage")]
        public QwenAITokenUsage Usage { get; set; } = new QwenAITokenUsage();
    }

    /// <summary>
    /// Represents a single choice in the QwenAI completion response.
    /// </summary>
    public class QwenAIChoice
    {
        /// <summary>
        /// Gets or sets the message containing the completion text.
        /// </summary>
        [JsonProperty("message")]
        public QwenAIMessage Message { get; set; } = new QwenAIMessage();

        /// <summary>
        /// Gets or sets the reason why the completion finished.
        /// </summary>
        [JsonProperty("finish_reason")]
        public string FinishReason { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a message in the QwenAI API response.
    /// </summary>
    public class QwenAIMessage
    {
        /// <summary>
        /// Gets or sets the role of the message sender (e.g., "assistant", "user").
        /// </summary>
        [JsonProperty("role")]
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents token usage information in the QwenAI API response.
    /// </summary>
    public class QwenAITokenUsage
    {
        /// <summary>
        /// Gets or sets the number of tokens in the prompt.
        /// </summary>
        [JsonProperty("prompt_tokens")]
        public int PromptTokens { get; set; }

        /// <summary>
        /// Gets or sets the number of tokens in the completion.
        /// </summary>
        [JsonProperty("completion_tokens")]
        public int CompletionTokens { get; set; }

        /// <summary>
        /// Gets or sets the total number of tokens used.
        /// </summary>
        [JsonProperty("total_tokens")]
        public int TotalTokens { get; set; }
    }
}
