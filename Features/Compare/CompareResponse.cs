// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Features.Compare
{
    using ChatbotBenchmarkAPI.Models.Response;

    /// <summary>
    /// Represents the response containing comparison results from two AI providers.
    /// </summary>
    public class CompareResponse
    {
        /// <summary>
        /// Gets or sets the response result from the left provider.
        /// </summary>
        public ProviderResult LeftResult { get; set; } = new ProviderResult();

        /// <summary>
        /// Gets or sets the response result from the right provider.
        /// </summary>
        public ProviderResult RightResult { get; set; } = new ProviderResult();
    }
}
