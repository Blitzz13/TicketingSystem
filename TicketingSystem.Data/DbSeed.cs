using System.Linq;

namespace TicketingSystem.Data
{
	public class DbSeed
	{
		private static TicketingSystemDbContext _context;

		public static void Seed(TicketingSystemDbContext context)
		{
			var user = new User()
			{
				Username = "test1",
				Password = "123",
				FirstName = "test",
				LastName = "testenov",
				Email = "test1@test.test"
			};

			var user1 = new User()
			{
				Username = "test2",
				Password = "123",
				FirstName = "test",
				LastName = "testenov",
				Email = "test2@test.test"
			};


			var user2 = new User()
			{
				Username = "test3",
				Password = "123",
				FirstName = "test",
				LastName = "testenov",
				Email = "test3@test.test"
			};


			var user3 = new User()
			{
				Username = "test4",
				Password = "123",
				FirstName = "test",
				LastName = "testenov",
				Email = "test4@test.test"
			};


			var user4 = new User()
			{
				Username = "test5",
				Password = "123",
				FirstName = "test",
				LastName = "testenov",
				Email = "test5@test.test"
			};

			_context = new Data.TicketingSystemDbContext();
			_context.AddRange(user,user1,user2,user3,user4);
			_context.SaveChanges();
		}
		
	}
}
