namespace TicketingSystem.Services
{
	public class ProjectModel
	{
		public ProjectModel(string title, string description)
		{
			Title = title;
			Description = description;
		}

		public string Title { get; set; }

		public string Description { get; set; }
	}
}