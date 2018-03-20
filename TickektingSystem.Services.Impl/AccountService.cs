using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using TickektingSystem.Data;
using TicketingSystem.Data.Models;
using DATA = TicketingSystem.Data;

namespace TickektingSystem.Services.Impl
{
	public class AccountService : IAccountService
	{
		private readonly TicketingSystemDbContext _context;

		public AccountService()
		{
			_context = new TicketingSystemDbContext();
		}

		public LoginResult Login(string userName, string password)
		{
			throw new System.NotImplementedException();
		}

		public RegisterResult Register(RegisterModel registerModel)
		{
			var validationPattern = Regex.Escape(@"^(("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+\/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))((\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$");
			if (string.IsNullOrEmpty(registerModel.FirstName))
			{
				return new RegisterResult
				{
					InvalidFirstName = true
				};
			}

			if (string.IsNullOrEmpty(registerModel.LastName))
			{
				return new RegisterResult
				{
					InvalidLastName = true
				};
			}

			if (string.IsNullOrEmpty(registerModel.UserName))
			{
				return new RegisterResult
				{
					InvalidFirstName = true
				};
			}

			if (_context.Users.Any(u => u.Username == registerModel.UserName))
			{
				return new RegisterResult
				{
					UsernameTaken = true
				};
			}

			if (string.IsNullOrEmpty(registerModel.FirstName))
			{
				return new RegisterResult
				{
					InvalidFirstName = true
				};
				
			}

			if (Regex.Match(registerModel.Email,validationPattern).Success)
			{
				return new RegisterResult()
				{
					IncorrectEmailFormat = true
				};
			}

			if (registerModel.UserName.Length < 3)
			{
				return new RegisterResult()
				{
					IvalidUsernameSize = true
				};
			}

			string password = HashPassword(registerModel);

			DATA.Models.User user = new DATA.Models.User
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
			

			return new RegisterResult
			{
				IsSuccessful = true
			};
		}

		public string HashPassword(RegisterModel registerModel)
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
