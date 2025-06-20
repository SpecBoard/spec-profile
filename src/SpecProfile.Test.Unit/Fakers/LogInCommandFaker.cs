using AutoBogus;
using SpecProfile.Commands;

namespace SpecProfile.Test.Unit.Fakers
{
	public class LogInCommandFaker : AutoFaker<LogInCommand>
	{
		public LogInCommandFaker UserName(string? userName)
		{
			RuleFor(c => c.User, userName);

			return this;
		}
	}
}
