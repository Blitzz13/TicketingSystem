using System.Collections.Generic;

namespace TicketingSystem.Services
{
	public interface IProjectService
	{
		int Create(CreateProjectModel model);

		IEnumerable<Project> Get();

		void Delete(int projectId);

		Project GetByName(string projectName);

		Project GetById(int projectId);
	}
}
