namespace TicketingSystem.Services
{
	public class CreateProjectModel
	{
		public CreateProjectModel(string title, string description)
		{
			Title = title;
			Description = description;
		}

		public string Title { get; set; }

		public string Description { get; set; }

		public int UserId { get; set; }
	}
}