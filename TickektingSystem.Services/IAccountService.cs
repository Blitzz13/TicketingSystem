namespace TickektingSystem.Services
{
	public interface IAccountService
	{
		LoginResult Login(string userName, string password);

		RegisterResult Register(RegisterModel registerModel);

		string HashPassword(RegisterModel registerModel);
	}
}
