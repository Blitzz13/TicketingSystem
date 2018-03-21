namespace TicketingSystem.Data
{
	public class File
	{
		public int Id { get; set; }

		public byte[] Content { get; set; }

		public int Name { get; set; }

		public int? TicketId { get; set; }

		public Ticket Ticket { get; set; }

		public int? MessageId { get; set; }

		public Message Message { get; set; }
	}
}
