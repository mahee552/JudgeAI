// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Features.Compare
{
    using ChatbotBenchmarkAPI.Models.Request;

    /// <summary>
    /// Represents a request to compare responses between two AI providers.
    /// </summary>
    public class CompareRequest
    {
        /// <summary>
        /// Gets or sets the common prompt or input text to be used for the AI model call.
        /// </summary>
        public List<Message> Messages { get; set; } = new List<Message>();

        /// <summary>
        /// Gets or sets the provider selection configuration for the left side comparison.
        /// </summary>
        public ProviderSelection LeftProvider { get; set; } = new ProviderSelection();

        /// <summary>
        /// Gets or sets the provider selection configuration for the right side comparison.
        /// </summary>
        public ProviderSelection RightProvider { get; set; } = new ProviderSelection();

        /// <summary>
        /// Gets or sets the configuration options such as temperature, whether to remember chat history and tokens limit.
        /// </summary>
        public ChatRequestSettings ChatRequestSettings { get; set; } = new ChatRequestSettings();
    }

    /// <summary>
    /// Represents the configuration for selecting a specific AI provider and model.
    /// </summary>
    public class ProviderSelection
    {
        /// <summary>
        /// Gets or sets the name of the AI provider (e.g., "OpenAI", "Deepseek").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the specific model identifier for the selected provider (e.g., "gpt-4", "R1", "gpt-3.5-turbo").
        /// </summary>
        public string Model { get; set; } = string.Empty;
    }
}
