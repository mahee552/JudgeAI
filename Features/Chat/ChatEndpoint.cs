// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Features.Chat
{
    using ChatbotBenchmarkAPI.Business.Validation.RequestValidation;
    using ChatbotBenchmarkAPI.Exceptions;
    using ChatbotBenchmarkAPI.Infrastructure.Services.Interfaces;
    using ChatbotBenchmarkAPI.Models.Response;
    using FastEndpoints;

    /// <summary>
    /// Represents an API endpoint for handling chat requests.
    /// </summary>
    /// <remarks>
    /// This endpoint processes chat messages and interacts with AI providers.
    /// </remarks>
    public class ChatEndpoint : Endpoint<ChatRequest, ProviderResult>
    {
        private readonly IAIProviderFactory _providerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatEndpoint"/> class.
        /// </summary>
        /// <param name="providerFactory">IAIProviderFactory.</param>
        public ChatEndpoint(IAIProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        /// <inheritdoc/>
        public override void Configure()
        {
            Post("/api/chat");
            AllowAnonymous();
            Validator<ChatRequestValidator>();
        }

        /// <inheritdoc/>
        public override async Task HandleAsync(ChatRequest request, CancellationToken ct)
        {
            try
            {
                var providerService = _providerFactory.GetProviderService(request.Provider);

                if (request.ChatRequestSettings.RememberHistory)
                {
                    request.Messages = request.Messages.Count > 5 ? request.Messages.TakeLast(5).ToList() : request.Messages;
                }

                if (!request.ChatRequestSettings.Stream)
                {
                    var response = await providerService.CallModelAsync(request.Model, request.Messages, request.ChatRequestSettings);
                    await SendAsync(response, cancellation: ct);
                }
                else
                {
                    HttpResponse httpResponse = HttpContext.Response;
                    await providerService.StreamModelResponseAsync(request.Model, request.Messages, request.ChatRequestSettings, httpResponse);
                }
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
