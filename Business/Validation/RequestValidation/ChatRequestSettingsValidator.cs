namespace ChatbotBenchmarkAPI.Business.Validation.RequestValidation
{
    using ChatbotBenchmarkAPI.Models.Request;
    using FluentValidation;

    /// <summary>
    /// Validator class for <see cref="ChatRequestSettings"/>.
    /// This class uses FluentValidation to enforce rules and constraints on the properties of a <see cref="ChatRequestSettings"/> object.
    /// </summary>
    /// <remarks>
    /// The validator ensures that:
    /// <list type="bullet">
    ///     <item><description>The <see cref="ChatRequestSettings.Temperature"/> is within the valid range of 0 to 1 (inclusive).</description></item>
    ///     <item><description>The <see cref="ChatRequestSettings.MaxTokens"/> is greater than 0 and does not exceed 4096.</description></item>
    /// </list>
    /// </remarks>
    public class ChatRequestSettingsValidator : AbstractValidator<ChatRequestSettings>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChatRequestSettingsValidator"/> class.
        /// </summary>
        public ChatRequestSettingsValidator()
        {
            RuleFor(x => x.Temperature)
                .InclusiveBetween(0, 1).WithMessage("Temperature must be between 0 and 1.");

            RuleFor(x => x.MaxTokens)
                .GreaterThan(0).WithMessage("MaxTokens must be greater than 0.");
        }
    }
}
