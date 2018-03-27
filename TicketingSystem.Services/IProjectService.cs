namespace TicketingSystem.Services
{
	public interface IProjectService
	{
		void CreateProject(int? userId, ProjectModel projectModel);

		void ViewProject(string projectName,int userId);

		void DeleteProject(string projectName);
	} 
}
