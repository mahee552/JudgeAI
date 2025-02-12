// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Models.CompletionResponses
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the response from the Gemini completion API.
    /// </summary>
    public class GeminiCompletionResponse
    {
        /// <summary>
        /// Gets or sets the list of candidate responses.
        /// </summary>
        [JsonProperty("candidates")]
        public List<GeminiCandidate> Candidates { get; set; } = new List<GeminiCandidate>();

        /// <summary>
        /// Gets or sets the metadata about the usage of the API.
        /// </summary>
        [JsonProperty("usageMetadata")]
        public GeminiUsageMetadata UsageMetadata { get; set; } = new GeminiUsageMetadata();
    }

    /// <summary>
    /// Represents an individual candidate response.
    /// </summary>
    public class GeminiCandidate
    {
        /// <summary>
        /// Gets or sets the content of the candidate response.
        /// </summary>
        [JsonProperty("content")]
        public GeminiContent Content { get; set; } = new GeminiContent();
    }

    /// <summary>
    /// Represents the content of a candidate response, which may consist of multiple parts.
    /// </summary>
    public class GeminiContent
    {
        /// <summary>
        /// Gets or sets the list of parts that make up the content.
        /// </summary>
        [JsonProperty("parts")]
        public List<GeminiPart> Parts { get; set; } = new List<GeminiPart>();
    }

    /// <summary>
    /// Represents an individual part of the content.
    /// </summary>
    public class GeminiPart
    {
        /// <summary>
        /// Gets or sets the text of the part.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;
    }

    /// <summary>
    /// Contains metadata about the usage of the API, including token counts.
    /// </summary>
    public class GeminiUsageMetadata
    {
        /// <summary>
        /// Gets or sets the number of tokens in the prompt.
        /// </summary>
        [JsonProperty("promptTokenCount")]
        public int PromptTokenCount { get; set; }

        /// <summary>
        /// Gets or sets the number of tokens in the candidates.
        /// </summary>
        [JsonProperty("candidatesTokenCount")]
        public int CandidatesTokenCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of tokens used.
        /// </summary>
        [JsonProperty("totalTokenCount")]
        public int TotalTokenCount { get; set; }
    }
}
