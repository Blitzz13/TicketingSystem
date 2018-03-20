using System.Collections.Generic;

namespace TicketingSystem.Data.Models
{
	public class User
	{
		public int Id { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }

		public Role Role { get; set; }

		public string Email { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public ICollection<Message> Messages { get; set; } = new List<Message>();
	}
}
