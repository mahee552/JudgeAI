// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Business.Validation.RequestValidation
{
    using ChatbotBenchmarkAPI.Features.Chat;
    using ChatbotBenchmarkAPI.Models.Request;
    using FluentValidation;

    /// <summary>
    /// Validator class for <see cref="ChatRequest"/>.
    /// This class uses FluentValidation to enforce rules and constraints on the properties of a <see cref="ChatRequest"/> object.
    /// </summary>
    /// <remarks>
    /// The validator ensures that:
    /// <list type="bullet">
    ///     <item><description>The <see cref="ChatRequest.Provider"/> is not empty and does not exceed 50 characters.</description></item>
    ///     <item><description>The <see cref="ChatRequest.Model"/> is not empty and does not exceed 50 characters.</description></item>
    ///     <item><description>The <see cref="ChatRequest.Messages"/> list is not empty and contains at least one valid <see cref="Message"/>.</description></item>
    ///     <item><description>The <see cref="ChatRequest.ChatRequestSettings"/> contains valid configuration options.</description></item>
    /// </list>
    /// </remarks>
    public class ChatRequestValidator : AbstractValidator<ChatRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChatRequestValidator"/> class.
        /// </summary>
        public ChatRequestValidator()
        {
            RuleFor(x => x.Provider)
                .NotEmpty().WithMessage("Provider is required.")
                .MaximumLength(50).WithMessage("Provider name must not exceed 50 characters.");

            RuleFor(x => x.Model)
                .NotEmpty().WithMessage("Model is required.")
                .MaximumLength(50).WithMessage("Model name must not exceed 50 characters.");

            RuleFor(x => x.Messages)
                .NotEmpty().WithMessage("At least one message is required.")
                .Must(messages => messages.Count > 0).WithMessage("At least one message is required.")
                .ForEach(messageRule =>
                {
                    messageRule.SetValidator(new MessageValidator());
                });

            RuleFor(x => x.ChatRequestSettings)
                .SetValidator(new ChatRequestSettingsValidator());
        }
    }
}
