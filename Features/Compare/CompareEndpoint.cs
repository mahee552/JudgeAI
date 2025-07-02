// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Features.Compare
{
    using ChatbotBenchmarkAPI.Business.Validation.RequestValidation;
    using ChatbotBenchmarkAPI.Exceptions;
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
            Post("/api/compare");
            AllowAnonymous();
            Validator<CompareRequestValidator>();
        }

        /// <inheritdoc/>
        public override async Task HandleAsync(CompareRequest req, CancellationToken ct)
        {
            try
            {
                // Resolve provider services using the factory.
                var leftProviderService = _providerFactory.GetProviderService(req.LeftProvider.Name);
                var rightProviderService = _providerFactory.GetProviderService(req.RightProvider.Name);

                if (req.ChatRequestSettings.RememberHistory)
                {
                    req.Messages = req.Messages.Count > 5 ? req.Messages.TakeLast(5).ToList() : req.Messages;
                }

                if (req.ChatRequestSettings.Stream)
                {
                    HttpResponse response = HttpContext.Response;
                    response.ContentType = "application/json";
                    response.Headers.CacheControl = "no-cache";
                    response.Headers.Connection = "keep-alive";

                    // Stream left provider response
                    await leftProviderService.StreamModelResponseAsync(req.LeftProvider.Model, req.Messages, req.ChatRequestSettings, response);

                    // Stream right provider response
                    await rightProviderService.StreamModelResponseAsync(req.RightProvider.Model, req.Messages, req.ChatRequestSettings, response);

                    return;
                }

                var leftTask = leftProviderService.CallModelAsync(req.LeftProvider.Model, req.Messages, req.ChatRequestSettings);
                var rightTask = rightProviderService.CallModelAsync(req.RightProvider.Model, req.Messages, req.ChatRequestSettings);
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
