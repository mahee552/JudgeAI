// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Models.Configurations
{
    /// <summary>
    /// Configuration class for AI provider endpoints.
    /// </summary>
    public class AIEndpointsConfig
    {
        /// <summary>
        /// Gets or sets Dictionary containing AI provider configurations, where the key is the provider name.
        /// </summary>
        public Dictionary<string, ProviderConfig> Providers { get; set; } = new Dictionary<string, ProviderConfig>();
    }

    /// <summary>
    /// Represents the configuration for an AI provider.
    /// </summary>
    public class ProviderConfig
    {
        /// <summary>
        /// Gets or sets the base URL of the AI provider's API.
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Dictionary of API endpoints for the AI provider, where the key is the endpoint name and the value is the corresponding URL path.
        /// </summary>
        public Dictionary<string, string> Endpoints { get; set; } = new Dictionary<string, string>();
    }
}
