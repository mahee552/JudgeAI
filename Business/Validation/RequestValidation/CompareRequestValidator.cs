// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Business.Validation.RequestValidation
{
    using ChatbotBenchmarkAPI.Features.Compare;
    using FluentValidation;

    /// <summary>
    /// Validator for the <see cref="CompareRequest"/> model.
    /// Ensures that the required properties are valid before processing the request.
    /// </summary>
    public class CompareRequestValidator : AbstractValidator<CompareRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompareRequestValidator"/> class.
        /// </summary>
        public CompareRequestValidator()
        {
            RuleFor(x => x.Messages)
                .NotEmpty().WithMessage("Prompt is required.")
                .Must(messages => messages.Count > 0).WithMessage("At least one message is required.")
                .ForEach(messageRule =>
                {
                    messageRule.SetValidator(new MessageValidator());
                });

            RuleFor(x => x.LeftProvider)
                .NotNull().WithMessage("Left provider is required.");

            RuleFor(x => x.RightProvider)
                .NotNull().WithMessage("Right provider is required.");

            RuleFor(x => x.LeftProvider.Name)
                .NotEmpty().WithMessage("Left provider name is required.");

            RuleFor(x => x.RightProvider.Name)
                .NotEmpty().WithMessage("Right provider name is required.");

            RuleFor(x => x.LeftProvider.Model)
                .NotEmpty().WithMessage("Left provider model is required.");

            RuleFor(x => x.RightProvider.Model)
                .NotEmpty().WithMessage("Right provider model is required.")
                .NotEqual(x => x.LeftProvider.Model).WithMessage("Left and right models must be different.");

            RuleFor(x => x.ChatRequestSettings)
                .SetValidator(new ChatRequestSettingsValidator());
        }
    }
}
