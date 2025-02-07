// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Features.Compare
{
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

    /// <summary>
    /// Represents the detailed result of an AI provider's response including performance metrics.
    /// </summary>
    public class ProviderResult
    {
        /// <summary>
        /// Gets or sets the response message returned by the AI model.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total number of tokens used, including both request and response.
        /// </summary>
        public int TotalTokens { get; set; }

        /// <summary>
        /// Gets or sets the calculated cost for the request.
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// Gets or sets the time taken in milliseconds to complete the API call.
        /// </summary>
        public long TimeTakenMs { get; set; }
    }
}
