using KurrentDB.Client;
using Microsoft.Extensions.Logging;
using SpecProfile.Domain;
using SpecProfile.Events;
using STrain.Eventing.Api;
using System.Text.Json;

namespace SpecProfile.Application.Repositories
{
	public interface IUserRepository
	{
		Task<UserAggregateRoot?> GetAsync(string username, CancellationToken cancellationToken);
		Task SaveAsync(IAggregateRoot<UserState> aggregateRoot, CancellationToken cancellationToken);
	}

	public class UserRepository : IUserRepository
	{
		private readonly string _category = "profile";

		private readonly KurrentDBClient _client;
		private readonly ILogger<UserRepository> _logger;

		public UserRepository(KurrentDBClient client, ILogger<UserRepository> logger)
		{
			_client = client;
			_logger = logger;
		}

		public async Task<UserAggregateRoot?> GetAsync(string username, CancellationToken cancellationToken)
		{
			_logger.LogDebug("Creating state");

			var streamResult = _client.ReadStreamAsync(Direction.Forwards, $"{_category}-{username}", StreamPosition.Start, cancellationToken: cancellationToken);

			if (await streamResult.ReadState == ReadState.StreamNotFound) return null;

			var result = new UserAggregateRoot();
			await streamResult.ForEachAwaitWithCancellationAsync(async (e, ct) =>
			{
				var @event = await e.DeserializeAsync(ct);
				switch (@event)
				{
					case UserCreatedEvent userCreated:
						_logger.LogDebug("Applying {EventType} event", userCreated.GetType());
						result.Apply(userCreated);
						break;
					case LoggedInEvent loggedIn:
						_logger.LogDebug("Applying {EventType} event", loggedIn.GetType());
						result.Apply(loggedIn);
						break;
					default:
						_logger.LogWarning("Unknown event type: {EventType}", @event.GetEventType());
						break;
				}
			}, cancellationToken);

			_logger.LogDebug("State has been created");
			return result;
		}

		public async Task SaveAsync(IAggregateRoot<UserState> aggregateRoot, CancellationToken cancellationToken)
		{
			_logger.LogDebug("Saving events");
			var stream = $"{_category}-{aggregateRoot.State.Name}";
			var data = aggregateRoot.UncommitedEvents.Select(e => new EventData(Uuid.NewUuid(), e.GetEventType(), JsonSerializer.SerializeToUtf8Bytes((dynamic)e)));

			_logger.LogTrace("Stream: {Stream}", stream);
			_logger.LogTrace("Content: {@Data}", data);

			await _client.AppendToStreamAsync(stream, StreamState.Any, data, cancellationToken: cancellationToken);
			_logger.LogDebug("{EventCount} events have been saved", data.Count());
		}
	}

	file static class UserRepositoryExtensions
	{
		public static async Task<IEvent> DeserializeAsync(this ResolvedEvent @event, CancellationToken cancellationToken)
		{
			await using var stream = new MemoryStream(@event.Event.Data.ToArray());
			var type = Type.GetType(@event.Event.EventType);

			return (IEvent)await JsonSerializer.DeserializeAsync(stream, type, cancellationToken: cancellationToken);
		}
	}
}
