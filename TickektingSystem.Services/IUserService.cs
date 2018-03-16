namespace TickektingSystem.Services
{
	public interface IUserService
	{
		bool IsSupport(int id);

		bool IsUser(int id);

		bool IsAdministrator(int id);
	}
}
