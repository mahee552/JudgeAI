namespace ChatbotBenchmarkAPI.Models.Response
{
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
        /// Gets or sets the time taken to complete the API call.
        /// </summary>
        public string TimeTaken { get; set; } = string.Empty;
    }
}
