namespace TicketingSystem.Services
{
	public interface IAccountService
	{
		LoginResult Login(string userName, string password);

		void Register(RegisterModel registerModel);

		void ApproveAccounts(string username);

		void EditUser(UserEditModel userEditModel, int commandNum);
	}
}
