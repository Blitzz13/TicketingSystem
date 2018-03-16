using TicketingSystem.Data;

namespace TickektingSystem.Services.Impl
{
	public class UserService : IUserService
	{
		private readonly TicketingSystemDbContext _context;

		public UserService()
		{
			_context = new TicketingSystemDbContext();
		}

		public bool IsAdministrator(int id)
		{
			throw new System.NotImplementedException();
		}

		public bool IsSupport(int id)
		{
			throw new System.NotImplementedException();
		}

		public bool IsUser(int id)
		{
			throw new System.NotImplementedException();
		}
	}
}
