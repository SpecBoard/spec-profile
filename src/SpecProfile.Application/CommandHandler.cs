using Microsoft.Extensions.Logging;
using SpecProfile.Application.Repositories;
using SpecProfile.Commands;
using STrain;

namespace SpecProfile.Application
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
            var user = await _repository.GetAsync(command.User, cancellationToken);
            user.LogIn(command.User);
            await _repository.SaveAsync(user, cancellationToken);
            _logger.LogInformation("User has been logged-in");
        }
    }
}
