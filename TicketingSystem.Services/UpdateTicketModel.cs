namespace TicketingSystem.Services
{
	public class UpdateTicketModel
	{
		public string ProjectName { get; set; }

		public int Id { get; set; }

		public string Type { get; set; }

		public string State { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public int Files { get; set; }
	}
}
