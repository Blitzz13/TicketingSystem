using System.Linq;

namespace TicketingSystem.Services.Impl
{
	public class ProjectService : IProjectService
	{

		private readonly Data.TicketingSystemDbContext _context;

		public ProjectService()
		{
			_context = new Data.TicketingSystemDbContext();
		}

		public void CreateProject(int? userId, ProjectModel projectModel)
		{
			if (userId == null)
			{
				throw new ServiceException("You are not logged in.");
			}

			var user = _context.Users.FirstOrDefault(u => u.Id == userId);

			if (user.Role == Data.Role.Administrator)
			{
				var project = new Data.Project
				{
					Name = projectModel.Title,
					Description = projectModel.Description
				};

				_context.Add(project);
				_context.SaveChanges();
			}
			else
			{
				throw new ServiceException("You have to be administrator to use this command.");
			}
		}

		public void ViewProject(int projectName)
		{
			throw new System.NotImplementedException();
		}

		public void DeleteProject(int projectId)
		{
			throw new System.NotImplementedException();
		}
	}
}
