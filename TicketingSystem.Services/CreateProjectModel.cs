namespace TicketingSystem.Services
{
	public class CreateProjectModel
	{
		public CreateProjectModel(string title, string description,int userId)
		{
			Title = title;
			Description = description;
			UserId = userId;
		}

		public string Title { get; set; }

		public string Description { get; set; }

		public int UserId { get; set; }
	}
}