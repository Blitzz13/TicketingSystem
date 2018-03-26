namespace TicketingSystem.Services
{
	public interface IAccountService
	{
		LoginResult Login(string userName, string password);

		void Register(RegisterModel registerModel);

		void ApproveAccounts();

		void EditUser(string username);
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD

		void CreateTicket(TicketModel ticketModel);
		void CreateProject(int? userId, ProjectModel projectModel);

		void CreateProject(int userId, ProjectModel projectModel);

=======
>>>>>>> parent of 28b8674... Backup commit
=======
>>>>>>> parent of 28b8674... Backup commit
=======
>>>>>>> parent of 28b8674... Backup commit
=======
>>>>>>> parent of 28b8674... Backup commit
=======
>>>>>>> parent of 28b8674... Backup commit
	}
}
