namespace TickektingSystem.Services
{
	public interface IAccountService
	{
		LogOnResult Logon(string userName, string password);

		RegisterResult Register(RegisterModel registerModel);
	}
}
