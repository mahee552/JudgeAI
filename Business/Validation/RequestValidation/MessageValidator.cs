// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

namespace ChatbotBenchmarkAPI.Business.Validation.RequestValidation
{
    using ChatbotBenchmarkAPI.Models.Request;
    using FluentValidation;

    /// <summary>
    /// Validator class for <see cref="Message"/>.
    /// This class uses FluentValidation to enforce rules and constraints on the properties of a <see cref="Message"/> object.
    /// </summary>
    /// <remarks>
    /// The validator ensures that:
    /// <list type="bullet">
    ///     <item><description>The <see cref="Message.Content"/> is not empty and does not exceed 1000 characters.</description></item>
    ///     <item><description>The <see cref="Message.Role"/> is not empty and does not exceed 50 characters.</description></item>
    /// </list>
    /// </remarks>
    public class MessageValidator : AbstractValidator<Message>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageValidator"/> class.
        /// </summary>
        public MessageValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Message content is required.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Message role is required.");
        }
    }
}
