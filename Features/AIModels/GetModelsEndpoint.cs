// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Features.AIModels
{
    using FastEndpoints;

    /// <summary>
    /// Endpoint to fetch available ai models.
    /// </summary>
    public class GetModelsEndpoint : EndpointWithoutRequest<List<string>>
    {
        /// <inheritdoc/>
        public override void Configure()
        {
            Get("/models");
            AllowAnonymous();
        }

        /// <inheritdoc/>
        public override async Task HandleAsync(CancellationToken ct)
        {
            try
            {
                List<string> response = GetAvailableModels();

                await SendAsync(response, cancellation: ct);
            }
            catch (Exception ex)
            {
                string message = "Error fetching avaliable models";
                Logger.LogError(ex, message: message);
            }
        }

        /// <summary>
        /// Retrieves a list of available models.
        /// </summary>
        /// <returns>
        /// A list of strings representing the names or identifiers of available models.
        /// </returns>
        /// <remarks>
        /// This method is typically used to fetch the available model names that can be selected or used in the application.
        /// </remarks>
        internal static List<string> GetAvailableModels()
        {
            var models = new List<string>
        {
            "gpt-4",
            "gpt-3.5",
            "deepseek-chat",
            "claude-3",
            "mistral-7b",
            "gemini-pro",
        };
            return models;
        }
    }
}
