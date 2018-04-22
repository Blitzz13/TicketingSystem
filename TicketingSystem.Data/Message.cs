using System;
using System.Collections.Generic;

namespace TicketingSystem.Data
{
	public class Message
	{
		public int Id { get; set; }

		public DateTime PublishingDate { get; set; }

		public User User { get; set; }

		public int UserId { get; set; }

		public Ticket Ticket { get; set; }

		public int TicketId { get; set; }

		public StateMessage State { get; set; }

		public string Content { get; set; }

		public ICollection<File> Files { get; set; } = new List<File>();
	}
}
