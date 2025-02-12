// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Business.Validation
{
    using System.Globalization;

    /// <summary>
    /// Provides validation for AI models based on configuration settings.
    /// </summary>
    public class AIModelValidator
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AIModelValidator> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AIModelValidator"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration instance.</param>
        /// <param name="logger">The logger instance for logging errors.</param>
        public AIModelValidator(IConfiguration configuration, ILogger<AIModelValidator> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Validates if a given AI model is supported for the specified provider.
        /// </summary>
        /// <param name="provider">The AI provider (e.g., OpenAI, Gemini).</param>
        /// <param name="model">The model name to validate.</param>
        /// <returns><c>true</c> if the model is supported; otherwise, <c>false</c>.</returns>
        public bool IsModelSupported(string provider, string model)
        {
            try
            {
                string config = $"SupportedModels:{provider}";
                string models = _configuration[config]
                    ?? throw new KeyNotFoundException("OpenAIService: Models configuration not found");

                string[] supportedModels = models.ToLower(CultureInfo.InvariantCulture).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                return supportedModels != null && supportedModels.Contains(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while validating AI models");
                return false;
            }
        }
    }
}
