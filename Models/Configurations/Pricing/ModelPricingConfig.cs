// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Models.Configurations.Pricing
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the pricing structure for a model's token usage, including input and output rates.
    /// </summary>
    public class ModelPricingConfig
    {
        /// <summary>
        /// Gets or sets the price per 1,000 input tokens.
        /// </summary>
        /// <value>The decimal price rate for input tokens, measured per 1,000 tokens.</value>
        [JsonProperty("input_per_1k_tokens")]
        public decimal InputPer1KTokens { get; set; }

        /// <summary>
        /// Gets or sets the price per 1,000 output tokens.
        /// </summary>
        /// <value>The decimal price rate for output tokens, measured per 1,000 tokens.</value>
        [JsonProperty("output_per_1k_tokens")]
        public decimal OutputPer1KTokens { get; set; }
    }
}
