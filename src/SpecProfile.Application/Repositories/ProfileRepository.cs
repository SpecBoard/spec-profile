using SpecProfile.Domain;

namespace SpecProfile.Application.Repositories
{
	public interface IProfileRepository
	{
		Task<UserAggregateRoot> GetAsync(string username, CancellationToken cancellationToken);
		Task SaveAsync(UserAggregateRoot aggregateRoot, CancellationToken cancellationToken);
	}

	public class ProfileRepository : IProfileRepository
	{
		public Task<UserAggregateRoot> GetAsync(string username, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task SaveAsync(UserAggregateRoot aggregateRoot, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
