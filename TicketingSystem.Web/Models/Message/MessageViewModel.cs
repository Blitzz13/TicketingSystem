using System;

namespace TicketingSystem.Web.Models.Message
{
	public class MessageViewModel
	{
		public int Id { get; set; }

		public DateTime PublishingDate { get; set; }

		public int UserId { get; set; }

		public string Username { get; set; }

		public string State { get; set; }

		public string Content { get; set; }
	}
}
