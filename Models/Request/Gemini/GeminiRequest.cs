// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Models.Request.Gemini
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a request for the Gemini AI model.
    /// </summary>
    public class GeminiRequest
    {
        /// <summary>
        /// Gets or sets the contents of the request.
        /// </summary>
        [JsonProperty("contents")]
        public List<GeminiContent> Contents { get; set; } = new();

        /// <summary>
        /// Gets or sets the configuration options for the generation process.
        /// </summary>
        [JsonProperty("generationConfig")] // Add the attribute to control the casing
        public GenerationConfig GenerationConfig { get; set; } = new GenerationConfig();
    }

    /// <summary>
    /// Represents the content structure for the Gemini request.
    /// </summary>
    public class GeminiContent
    {
        /// <summary>
        /// Gets or sets the parts of the content.
        /// </summary>
        [JsonProperty("parts")]
        public List<GeminiPart> Parts { get; set; } = new();
    }

    /// <summary>
    /// Represents a part of the content in a Gemini request.
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
    /// Configuration options for the generation process, controlling aspects like randomness and maximum length.
    /// </summary>
    public class GenerationConfig
    {
        /// <summary>
        /// Gets or sets the randomness of the output. Higher values make the output more random, lower values make it more deterministic.
        /// Typical values are between 0.0 and 1.0.
        /// </summary>
        [JsonProperty("temperature", DefaultValueHandling = DefaultValueHandling.Ignore)] // Add the attribute to control the casing
        public double Temperature { get; set; } = 0.5;

        /// <summary>
        /// Gets or sets the length of the generated response in tokens.
        /// </summary>
        [JsonProperty("maxOutputTokens", DefaultValueHandling = DefaultValueHandling.Ignore)] // Add the attribute to control the casing
        public int MaxOutputTokens { get; set; } = 200;
    }
}
