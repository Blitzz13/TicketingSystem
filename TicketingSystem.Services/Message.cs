using System;

namespace TicketingSystem.Services
{
	public class Message
	{
		public int Id { get; set; }

		public DateTime PublishingDate { get; set; }

		public int UserId { get; set; }

		public string Username { get; set; }

		public string TicketTitle { get; set; }

		public int TicketId { get; set; }

		public int ProjectId { get; set; }

		public string State { get; set; }

		public string Content { get; set; }

		public int FileCount { get; set; }
	}
}
