namespace ChatbotBenchmarkAPI.Models.CompletionResponses
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the completion response from the Anthropic API.
    /// </summary>
    public class AnthropicCompletionResponse
    {
        /// <summary>
        /// Gets or sets the list of content blocks in the response.
        /// </summary>
        [JsonProperty("content")]
        public List<AnthropicContentBlock> Content { get; set; } = new List<AnthropicContentBlock>();

        /// <summary>
        /// Gets or sets the number of input tokens used.
        /// </summary>
        [JsonProperty("input_tokens")]
        public int? InputTokens { get; set; }

        /// <summary>
        /// Gets or sets the number of output tokens generated.
        /// </summary>
        [JsonProperty("output_tokens")]
        public int? OutputTokens { get; set; }
    }

    /// <summary>
    /// Represents a content block in the Anthropic API response.
    /// </summary>
    public class AnthropicContentBlock
    {
        /// <summary>
        /// Gets or sets the type of the content block.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the text content of the block.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;
    }
}
