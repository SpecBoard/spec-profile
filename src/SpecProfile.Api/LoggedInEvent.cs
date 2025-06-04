using STrain;

namespace SpecProfile.Api
{
	public record LoggedInEvent : Event
	{
		public required string Username { get; init; }
	}
}
