// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Business.Pricing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using ChatbotBenchmarkAPI.Models.Configurations.Pricing;
    using Newtonsoft.Json;

    /// <summary>
    /// Provides services for calculating costs based on token usage for different AI models and providers.
    /// </summary>
    public static class PricingService
    {
        /// <summary>
        /// Calculates the total cost for a given number of input and output tokens for a specific model.
        /// </summary>
        /// <param name="provider">The AI provider name (e.g., "OpenAI", "Anthropic").</param>
        /// <param name="modelName">The name of the AI model.</param>
        /// <param name="inputTokens">The number of input tokens used.</param>
        /// <param name="outputTokens">The number of output tokens generated.</param>
        /// <returns>The calculated cost in the pricing configuration's currency unit.</returns>
        /// <exception cref="ArgumentException">Thrown when pricing data is not found for the specified provider and model combination.</exception>
        public static decimal CalculateCost(string provider, string modelName, int inputTokens, int outputTokens)
        {
            string pricingFilePath = "pricing.json"; // Ensure this file is in the correct directory

            if (!File.Exists(pricingFilePath))
            {
                throw new FileNotFoundException("Pricing file not found!", pricingFilePath);
            }

            var json = File.ReadAllText(pricingFilePath);
            Dictionary<string, Dictionary<string, ModelPricingConfig>> pricingData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, ModelPricingConfig>>>(json) ?? throw new KeyNotFoundException();

            return pricingData.TryGetValue(provider, out var models) && models.TryGetValue(modelName, out var modelPricing)
                ? (inputTokens / 1000m * modelPricing.InputPer1KTokens) +
                       (outputTokens / 1000m * modelPricing.OutputPer1KTokens)
                : throw new ArgumentException("Model pricing not found!");
        }
    }
}
