namespace TicketingSystem.Services
{
	public class CreateTicketModel
	{
		public string TicketTitle { get; set; }

		public string TicketState { get; set; }

		public string TicketType { get; set; }

		public string TicketDescription { get; set; }

		public byte[] FileContent { get; set; }

		public string FileName { get; set; }

		public int ProjectId { get; set; }

		public int SubmitterId { get; set; }
	}
}
