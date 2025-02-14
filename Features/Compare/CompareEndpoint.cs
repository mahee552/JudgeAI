// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Features.Compare
{
    using ChatbotBenchmarkAPI.Business.Validation.RequestValidation;
    using ChatbotBenchmarkAPI.CustomExceptions;
    using ChatbotBenchmarkAPI.Infrastructure.Services.Interfaces;
    using FastEndpoints;

    /// <summary>
    /// Endpoint that handles comparison requests between two AI providers, accepting a CompareRequest and returning a CompareResponse.
    /// Processes parallel calls to different AI providers and aggregates their responses.
    /// </summary>
    public class CompareEndpoint : Endpoint<CompareRequest>
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
            Validator<CompareRequestValidator>();
        }

        /// <inheritdoc/>
        public override async Task HandleAsync(CompareRequest request, CancellationToken ct)
        {
            try
            {
                // Resolve provider services using the factory.
                var leftProviderService = _providerFactory.GetProviderService(request.LeftProvider.Name);
                var rightProviderService = _providerFactory.GetProviderService(request.RightProvider.Name);

                // Option 1: Run concurrently
                var leftTask = leftProviderService.CallModelAsync(request.LeftProvider.Model, request.Prompt);
                var rightTask = rightProviderService.CallModelAsync(request.RightProvider.Model, request.Prompt);
                await Task.WhenAll(leftTask, rightTask);

                await SendAsync(new CompareResponse { LeftResult = await leftTask, RightResult = await rightTask }, cancellation: ct);
            }
            catch (ModelNotSupportedException ex)
            {
                Logger.LogError(ex, ex.Message);
                AddError(ex.Message, "Request was cancelled.");
                ThrowIfAnyErrors(statusCode: StatusCodes.Status400BadRequest);
            }
            catch (HttpRequestException ex)
            {
                Logger.LogError(ex, ex.Message);
                AddError("An error occurred while communicating with the external API");
                ThrowIfAnyErrors(statusCode: StatusCodes.Status502BadGateway);
            }
            catch (TaskCanceledException ex) when (ct.IsCancellationRequested)
            {
                Logger.LogWarning(ex, "The request was canceled by the client.");
                AddError("The request was canceled by the client.", "Request was cancelled.");
                ThrowIfAnyErrors(statusCode: StatusCodes.Status499ClientClosedRequest);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                AddError("An unexpected error occurred.", errorCode: ex.Message);
                ThrowIfAnyErrors(statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
