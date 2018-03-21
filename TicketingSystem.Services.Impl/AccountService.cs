using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using DATA = TicketingSystem.Data;

namespace TicketingSystem.Services.Impl
{
	public class AccountService : IAccountService
	{
		private readonly DATA.TicketingSystemDbContext _context;

		public AccountService()
		{
			_context = new DATA.TicketingSystemDbContext();
		}

		#region  AccountService members

		public LoginResult Login(string userName, string password)
		{
			DATA.User user = _context.Users.FirstOrDefault(u => u.Username == userName);
			if (user == null)
			{

			}

			byte[] hashBytes = Convert.FromBase64String(user.Password);
			byte[] salt = new byte[16];
			Array.Copy(hashBytes, 0, salt, 0, 16);
			var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
			byte[] hash = pbkdf2.GetBytes(20);

			for (int i = 0; i < 20; i++)
			{
				if (hashBytes[i + 16] != hash[i])
				{
					Console.WriteLine("Wrong password!");
					return new LoginResult
					{

					};
				}
			}

			return new LoginResult
			{
				UserId = user.Id
			};
		}

		public void Register(RegisterModel registerModel)
		{
			if (string.IsNullOrEmpty(registerModel.FirstName))
			{
				throw new ServiceException("Your first name cannot be empty.");
			}

			if (string.IsNullOrEmpty(registerModel.LastName))
			{
				throw new ServiceException("Your last name cannot be empty.");
			}

			if (string.IsNullOrEmpty(registerModel.UserName))
			{
				throw new ServiceException("Your username cannot be empty.");
			}

			if (_context.Users.Any(u => u.Username == registerModel.UserName))
			{
				throw new ServiceException("The username you have chosen already exists.");
			}

			var regex = new Regex(@"^([0-9a-zA-Z_]([_+-.\w]*[0-9a-zA-Z_])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$/");
			Match match = regex.Match(registerModel.Email);
			if (!match.Success)
			{
				throw new ServiceException("The email you enterted is in incorrect format");
			}

			if (registerModel.UserName.Length < 3)
			{
				throw new ServiceException("The username should be more than 2 characters");
			}

			string password = HashPassword(registerModel);

			DATA.User user = new DATA.User
			{
				Username = registerModel.UserName,
				Password = password,
				Email = registerModel.Email,
				FirstName = registerModel.FirstName,
				LastName = registerModel.LastName,
				AccountState = registerModel.AccountState
			};

			_context.Add(user);
			_context.SaveChanges();
		}

		#endregion

		public static string HashPassword(RegisterModel registerModel)
		{
			byte[] salt;
			new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

			var pbkdf2 = new Rfc2898DeriveBytes(registerModel.Passowrd, salt, 10000);
			byte[] hash = pbkdf2.GetBytes(20);

			byte[] hashBytes = new byte[36];
			Array.Copy(salt, 0, hashBytes, 0, 16);
			Array.Copy(hash, 0, hashBytes, 16, 20);

			return Convert.ToBase64String(hashBytes);
		}
	}
}
