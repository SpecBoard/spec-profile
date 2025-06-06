using Microsoft.Extensions.Logging;
using SpecProfile.Application.Repositories;
using SpecProfile.Commands;
using SpecProfile.Domain;
using STrain;

namespace SpecProfile.Application.Performers
{
	public class LogInCommandPerformer : ICommandPerformer<LogInCommand>
	{
		private readonly IUserRepository _repository;
		private readonly ILogger<LogInCommandPerformer> _logger;

		public LogInCommandPerformer(IUserRepository repository, ILogger<LogInCommandPerformer> logger)
		{
			_repository = repository;
			_logger = logger;
		}

		public async Task PerformAsync(LogInCommand command, CancellationToken cancellationToken)
		{
			var user = await _repository.GetAsync(command.UserName, cancellationToken);
			if (user is null)
			{
				user = new UserAggregateRoot();
				user.Create(command.UserName);
				_logger.LogInformation("User has been created");
			}
			user.LogIn();
			await _repository.SaveAsync(user, cancellationToken);
			_logger.LogInformation("User has been logged-in");
		}
	}
}
