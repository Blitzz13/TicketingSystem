namespace TicketingSystem.Services
{
	public interface IUserService
	{
		LoginResult Login(string userName, string password);

		void Create(CreateUserModel model);

		void Approve(int userId);

		void Deny(int userId);

		void Update(int userId, UpdateUserModel model);

		void Delete(int userId);

		User GetByUsername(string userName);

		User GetByUserId(int userId);
	}
}
