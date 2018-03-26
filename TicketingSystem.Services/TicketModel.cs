namespace TicketingSystem.Services
{
	public class TicketModel
	{
		public string TicketTitle { get; set; }

		public string TicketState { get; set; }

		public string TicketType { get; set; }

		public string TicketDescription { get; set; }

		public byte[] FileContent { get; set; }

		public string FileName { get; set; }
	}
}
