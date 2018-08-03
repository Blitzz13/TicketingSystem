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
			if (_context.Projects.Any(p => p.Name == model.Title))
			{
				throw new ServiceException($"Project with name '{model.Title}' already exists.");
			}

			if (string.IsNullOrEmpty(model.Title) || string.IsNullOrWhiteSpace(model.Title))
			{
				throw new ServiceException("Title cannot be empty or just spaces.");
			}

			if (string.IsNullOrEmpty(model.Description) || string.IsNullOrWhiteSpace(model.Description))
			{
				throw new ServiceException("Description cannot be empty or just spaces.");
			}

			if (model.Title.Length < 4)
			{
				throw new ServiceException("Title cannot have less than 4 characters.");
			}

			if (model.Description.Length < 6)
			{
				throw new ServiceException("Description cannot have less than 6 characters.");
			}

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

		public Project GetById(int projectId)
		{
			DATA.Project project = _context.Projects.First(p => p.Id == projectId);

			return CreateProject(project);
		}
	}
}
