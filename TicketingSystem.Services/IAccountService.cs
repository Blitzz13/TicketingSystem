namespace TicketingSystem.Services
{
	public interface IAccountService
	{
		LoginResult Login(string userName, string password);

		void Register(RegisterModel registerModel);
	}
}
