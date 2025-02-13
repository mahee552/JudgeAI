namespace ChatbotBenchmarkAPI.Models.CompletionResponses
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the response from Mistral AI's completion API.
    /// </summary>
    public class MistralAICompletionResponse
    {
        /// <summary>
        /// Gets or sets the generated text from the model.
        /// </summary>
        [JsonProperty("generated_text")]
        public string GeneratedText { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the usage details, including token counts.
        /// </summary>
        [JsonProperty("usage")]
        public Usage TotalUsage { get; set; } = new Usage();

        /// <summary>
        /// Represents the usage details for the API call.
        /// </summary>
        public class Usage
        {
            /// <summary>
            /// Gets or sets the number of tokens in the input prompt.
            /// </summary>
            [JsonProperty("prompt_tokens")]
            public int PromptTokens { get; set; }

            /// <summary>
            /// Gets or sets the number of tokens in the generated completion.
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
}
