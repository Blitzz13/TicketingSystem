using System;
using System.Linq;
using System.Security.Cryptography;

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
				Password = HashPassword("123"),
				FirstName = "test",
				LastName = "testenov",
				Email = "test1@test.test"
			};

			var user1 = new User()
			{
				Username = "test2",
				Password = HashPassword("123"),
				FirstName = "test",
				LastName = "testenov",
				Email = "test2@test.test"
			};


			var user2 = new User()
			{
				Username = "test3",
				Password = HashPassword("123"),
				FirstName = "test",
				LastName = "testenov",
				Email = "test3@test.test"
			};


			var user3 = new User()
			{
				Username = "test4",
				Password = HashPassword("123"),
				FirstName = "test",
				LastName = "testenov",
				Email = "test4@test.test"
			};


			var user4 = new User()
			{
				Username = "test5",
				Password = HashPassword(HashPassword("123")),
				FirstName = "test",
				LastName = "testenov",
				Email = "test5@test.test"
			};

			var admin = new User()
			{
				Username = "admin",
				Password = HashPassword(HashPassword("123")),
				FirstName = "test",
				LastName = "testenov",
				Email = "admin@test.test",
				Role = AccountRole.Administrator,
				AccountState = AccountState.Aproved
			};

			var support = new User()
			{
				Username = "support",
				Password = HashPassword(HashPassword("123")),
				FirstName = "Support",
				LastName = "Supportev",
				Email = "support@test.test",
				Role = AccountRole.Support,
				AccountState = AccountState.Aproved
			};

			_context.AddRange(user,user1,user2,user3,user4,admin,support);
			_context.SaveChanges();
		}

		public static string HashPassword(string password)
		{
			byte[] salt;
			new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

			var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
			byte[] hash = pbkdf2.GetBytes(20);

			byte[] hashBytes = new byte[36];
			Array.Copy(salt, 0, hashBytes, 0, 16);
			Array.Copy(hash, 0, hashBytes, 16, 20);

			return Convert.ToBase64String(hashBytes);
		}
	}
}
