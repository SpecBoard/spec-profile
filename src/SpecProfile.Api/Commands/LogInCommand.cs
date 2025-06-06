using FluentValidation;
using STrain;

namespace SpecProfile.Commands
{
	public record LogInCommand : Command
	{
		public required string UserName { get; init; }
	}
}

namespace SpecProfile.Commands.Validators
{
	public class LogInCommandValidator : AbstractValidator<LogInCommand>
	{
		public LogInCommandValidator()
		{
			RuleFor(c => c.UserName)
				.NotEmpty();
		}
	}
}