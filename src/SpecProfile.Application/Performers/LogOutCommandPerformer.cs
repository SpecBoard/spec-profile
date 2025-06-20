using Microsoft.Extensions.Logging;
using SpecProfile.Application.Repositories;
using SpecProfile.Commands;
using STrain;

namespace SpecProfile.Application.Performers
{
	public class LogOutCommandPerformer : ICommandPerformer<LogOutCommand>
	{
		private readonly IUserRepository _repository;
		private readonly ILogger<LogOutCommandPerformer> _logger;

		public LogOutCommandPerformer(IUserRepository repository, ILogger<LogOutCommandPerformer> logger)
		{
			_repository = repository;
			_logger = logger;
		}

		public async Task PerformAsync(LogOutCommand command, CancellationToken cancellationToken)
		{
			var user = await _repository.GetAsync(command.User, cancellationToken);
			user.LogOut();
			await _repository.SaveAsync(user, cancellationToken);
		}
	}
}
