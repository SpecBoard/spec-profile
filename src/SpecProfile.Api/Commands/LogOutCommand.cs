using FluentValidation;
using STrain;

namespace SpecProfile.Commands
{
	public record LogOutCommand : Command
	{
		public required string User { get; init; }
	}
}

namespace SpecProfile.Commands.Validators
{
	public class LogOutCommandValidator : AbstractValidator<LogOutCommand>
	{
		public LogOutCommandValidator()
		{
			RuleFor(c => c.User).NotEmpty();
		}
	}
}
