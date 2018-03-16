using TicketingSystem.Data;

namespace TickektingSystem.Services.Impl
{
	public class AccountService : IAccountService
	{
		private readonly TicketingSystemDbContext _context;

		public AccountService()
		{
			_context = new TicketingSystemDbContext();
		}

		public LogOnResult Logon(string userName, string password)
		{
			throw new System.NotImplementedException();
		}

		public RegisterResult Register(RegisterModel registerModel)
		{
			throw new System.NotImplementedException();
		}
	}
}
