namespace TicketingSystem.Services
{
	public interface IProjectService
	{
		void CreateProject(int? userId, ProjectModel projectModel);

		void ViewProject(int projectName);

		void DeleteProject(int projectId);
	} 
}
