using System;
using System.Data;
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
				throw new ServiceException("The username you have entered is wrong.");
			}

			byte[] hashBytes, hash;
			UnhashPassword(password, user, out hashBytes, out hash);
			ValidatePassword(hashBytes, hash);

			if (user.AccountState == DATA.AccountState.Pending)
			{
				throw new ServiceException("Your account is not approved yet. Please try again later.");
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

			if (string.IsNullOrEmpty(registerModel.Email))
			{
				throw new ServiceException("The email cannot be empty");
			}

			var regex = new Regex(@"^([0-9a-zA-Z_]([_+-.\w]*[0-9a-zA-Z_])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
			Match match = regex.Match(registerModel.Email);
			if (!match.Success)
			{
				throw new ServiceException("The email you enterted is in incorrect format");
			}

			if (registerModel.UserName.Length < 3)
			{
				throw new ServiceException("The username should be more than 2 characters");
			}

			string password = HashPassword(registerModel.Passowrd);

			DATA.User user = new DATA.User
			{
				Username = registerModel.UserName,
				Password = password,
				Email = registerModel.Email,
				FirstName = registerModel.FirstName,
				LastName = registerModel.LastName,
				AccountState = DATA.AccountState.Pending
			};

			_context.Add(user);
			_context.SaveChanges();
		}

		public void ApproveAccounts(string username)
		{
			var accToApprove = _context.Users.FirstOrDefault(u => u.Username == username);

			if (accToApprove == null)
			{
				throw new ServiceException("No account found with that name.");
			}

			if (accToApprove.AccountState == DATA.AccountState.Aproved)
			{
				throw new ServiceException("This account has already been approved.");
			}

			accToApprove.AccountState = DATA.AccountState.Aproved;
			_context.SaveChanges();
		}

		public void EditUser(UserEditModel userEditModel, int commandNum)
		{
			var user = _context.Users.FirstOrDefault(u => u.Username == userEditModel.Username);
			if (user == null)
			{
				throw new ServiceException($"Account with username ({userEditModel.Username}) does not exist.");
			}

			Console.Write("Enter edit command: ");
			string[] command = Console.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);


			if (string.Join(" ", command).ToLower() == "change password")
			{

				string newPassword = HashPassword(userEditModel.Password);
				user.Password = newPassword;
				_context.SaveChanges();
			}
			else if (string.Join(" ", command).ToLower() == "change email")
			{
				user.Email = userEditModel.Email;
				_context.SaveChanges();
			}
			else if (string.Join(" ", command).ToLower() == "change first name")
			{
				user.FirstName = userEditModel.FirstName;
				_context.SaveChanges();
			}
			else if (string.Join(" ", command).ToLower() == "change last name")
			{
				user.LastName = userEditModel.LastName;
				_context.SaveChanges();
			}
			else if (string.Join(" ", command).ToLower() == "change role")
			{
				DATA.AccountRole role = (DATA.AccountRole)Enum.Parse(typeof(DATA.AccountRole), userEditModel.Role);
				if (Enum.IsDefined(typeof(DATA.AccountRole), userEditModel))
				{
					user.Role = role;
					_context.SaveChanges();
				}
				else
				{
					throw new ServiceException("You have chosen non existing role.");
				}

			}

		}

		#endregion

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

		private static void UnhashPassword(string password, DATA.User user, out byte[] hashBytes, out byte[] hash)
		{
			hashBytes = Convert.FromBase64String(user.Password);
			byte[] salt = new byte[16];
			Array.Copy(hashBytes, 0, salt, 0, 16);
			var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
			hash = pbkdf2.GetBytes(20);
		}

		private static void ValidatePassword(byte[] hashBytes, byte[] hash)
		{
			for (int i = 0; i < 20; i++)
			{
				if (hashBytes[i + 16] != hash[i])
				{
					throw new ServiceException("The password you have entered is wrong.");
				}
			}
		}

	}
}
