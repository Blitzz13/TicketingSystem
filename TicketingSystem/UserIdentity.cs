namespace TicketingSystem
{
	public class UserIdentity
	{
		public int UserId { get; set; }

		public bool IsAdministrator { get; set; }

		public bool IsSupport { get; set; }

		public bool IsClient { get; set; }
	}
}
