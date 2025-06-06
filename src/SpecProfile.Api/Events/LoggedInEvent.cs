using STrain;

namespace SpecProfile.Events
{
	public record LoggedInEvent : Event
	{
		public required string UserName { get; init; }
	}
}
