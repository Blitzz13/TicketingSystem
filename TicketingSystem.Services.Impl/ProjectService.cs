using System.Collections.Generic;
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

		public int Create(CreateProjectModel model)
		{
			var project = new DATA.Project
			{
				Name = model.Title,
				Description = model.Description,
				UserId = model.UserId
			};

			_context.Add(project);
			_context.SaveChanges();

			return project.Id;
		}

		public IEnumerable<Project> Get()
		{
			return _context.Projects.Select(CreateProject);
		}

		public void Delete(int projectId)
		{
			DATA.Project project = _context.Projects.First(p => p.Id == projectId);

			_context.Projects.Remove(project);
			_context.SaveChanges();
		}

		public static Project CreateProject(DATA.Project project)
		{
			return new Project()
			{
				Id = project.Id,
				Name = project.Name,
				Description = project.Description,
				UserId = project.UserId
			};
		}

		public Project GetByName(string projectName)
		{
			DATA.Project project = _context.Projects.First(p => p.Name == projectName);

			return CreateProject(project);
		} 
	}
}
