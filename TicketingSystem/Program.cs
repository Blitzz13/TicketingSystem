using TickektingSystem.Data;
using TickektingSystem.Services;
using TickektingSystem.Services.Impl;

namespace TicketingSystem
{
	class Program
	{
		static void Main(string[] args)
		{
			DbSeed.Seed(new Data.TicketingSystemDbContext());

			IAccountService accountService = new AccountService();


		}
	}
}
