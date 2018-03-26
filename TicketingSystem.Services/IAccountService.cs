using TicketingSystem.Data;

namespace TicketingSystem.Services
{
	public interface IAccountService
	{
		LoginResult Login(string userName, string password);

		void Register(RegisterModel registerModel);

		void ChangeRole(string username,int? loggedAccId,Role roleToChange);

		void ApproveAccounts();

		void EditUser(string username);
	}
}
