using System.Collections.Generic;

namespace TicketingSystem.Data
{
	public class Project
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public int UserId { get; set; }

		public User User { get; set; }

		public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
	}
}