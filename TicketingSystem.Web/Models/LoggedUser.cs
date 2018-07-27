namespace TicketingSystem.Web.Models
{
	public class LoggedUser
	{
		public int UserId { get; set; }

		public string Username { get; set; }

		public bool IsAdministrator { get; set; }

		public bool IsSupport { get; set; }

		public bool IsClient { get; set; }
	}
}
