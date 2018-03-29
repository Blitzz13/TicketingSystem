namespace TicketingSystem.Services
{
	public interface IUserService
	{
		LoginResult Login(string userName, string password);

		void Create(CreateUserModel model);

		void Approve(int userId);

		void Update(int userId, UpdateUserModel model);

		User GetByUsername(string userName);
	}
}
