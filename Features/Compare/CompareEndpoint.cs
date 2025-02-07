// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Features.Compare
{
    using ChatbotBenchmarkAPI.Infrastructure.Services.Interfaces;
    using FastEndpoints;

    /// <summary>
    /// Endpoint that handles comparison requests between two AI providers, accepting a CompareRequest and returning a CompareResponse.
    /// Processes parallel calls to different AI providers and aggregates their responses.
    /// </summary>
    public class CompareEndpoint : Endpoint<CompareRequest, CompareResponse>
    {
        private readonly IAIProviderFactory _providerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareEndpoint"/> class.
        /// </summary>
        /// <param name="providerFactory">IAIProviderFactory.</param>
        public CompareEndpoint(IAIProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        /// <inheritdoc/>
        public override void Configure()
        {
            Post("/compare");
            AllowAnonymous();
        }

        /// <inheritdoc/>
        public override async Task HandleAsync(CompareRequest request, CancellationToken ct)
        {
            try
            {
                // Resolve provider services using the factory.
                var leftProviderService = _providerFactory.GetProviderService(request.LeftProvider.Provider);
                var rightProviderService = _providerFactory.GetProviderService(request.RightProvider.Provider);

                // Option 1: Run concurrently
                var leftTask = leftProviderService.CallModelAsync(request.LeftProvider.Model, request.Prompt);
                var rightTask = rightProviderService.CallModelAsync(request.RightProvider.Model, request.Prompt);
                await Task.WhenAll(leftTask, rightTask);

                await SendAsync(new CompareResponse { LeftResult = await leftTask, RightResult = await rightTask }, cancellation: ct);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error while comparing models.");
                await SendAsync(new CompareResponse(), statusCode: StatusCodes.Status500InternalServerError, cancellation: ct);
            }
        }
    }
}
