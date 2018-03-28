using System;
using System.Linq;
using DATA = TicketingSystem.Data;

namespace TicketingSystem.Services.Impl
{
	public class ProjectService : IProjectService
	{

		private readonly DATA.TicketingSystemDbContext _context;

		public ProjectService()
		{
			_context = new DATA.TicketingSystemDbContext();
		}

		public void CreateProject(int? userId, ProjectModel projectModel)
		{
			if (userId == null)
			{
				throw new ServiceException("You are not logged in.");
			}

			var user = _context.Users.FirstOrDefault(u => u.Id == userId);

			if (user.Role != DATA.AccountRole.Administrator)
			{
				throw new ServiceException("You are not administator.");
			}

			if (user.Role == Data.AccountRole.Administrator)
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

		public void ViewProject(string projectName, int userId)
		{
			var project = _context.Projects.FirstOrDefault(p => p.Name == projectName);
			if (project == null)
			{
				throw new ServiceException($"No project with name {projectName} have been found.");
			}

			Console.WriteLine(projectName);
			Console.WriteLine(project.Description);

			//DATA.User user = _context.Users.FirstOrDefault(u => u.Id == userId);
			//View tickets
			//if (user.Role == DATA.AccountRole.Client)
			//{
			//	foreach (var ticket in project.Tickets.Where(a => a.Submitter == user))
			//	{

			//	}
			//}
		}

		public void DeleteProject(string projectName)
		{
			var project = _context.Projects.FirstOrDefault(p => p.Name == projectName);
			if (project == null)
			{
				throw new ServiceException($"No project with name {projectName} have been found.");
			}

			_context.Projects.Remove(project);
			_context.SaveChanges();
			Console.WriteLine($"Project {projectName} has been succsessfully deleted.");
		}
	}
}
