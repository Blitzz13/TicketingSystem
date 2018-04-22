using System;

namespace TicketingSystem.Services
{
	public class CreateMessageModel
	{
		public DateTime PublishingDate { get; set; }

		public int UserId { get; set; }

		public int TicketId { get; set; }

		public string State { get; set; }

		public string Content { get; set; }

		public byte[] FileContent { get; set; }

		public string FileName { get; set; }
	}
}
