using System;
using System.IO;
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

		public void ApproveAccounts()
		{
			Console.WriteLine("These accounts are to be processed:");
			foreach (var user in _context.Users.Where(u => u.AccountState == DATA.AccountState.Pending))
			{
				Console.WriteLine($"{user.Id} - {user.Username}");
			}

			string command;
			do
			{
				Console.Write("Enter user id to approve or 'stop': ");
				command = Console.ReadLine();
				if (command == "stop")
				{
					Console.WriteLine("You stopped approving");
					break;
				}

				int id = int.Parse(command);
				var accToApprove = _context.Users.FirstOrDefault(u => u.Id == id);

				while (accToApprove == null)
				{
					Console.WriteLine("There is no person with this id.");
					Console.Write("Enter user id to approve: ");
					id = int.Parse(Console.ReadLine());
					accToApprove = _context.Users.FirstOrDefault(u => u.Id == id);
				}

				if (accToApprove.AccountState == DATA.AccountState.Aproved)
				{
					Console.Write("This account is already approved. Do you want to stop approving (enter 'y' or 'n'): ");
					command = Console.ReadLine();
					if (command.ToLower() == "y")
					{
						break;
					}
				}
				else
				{
					Console.Write("Are you sure(enter 'y' or 'n' 'stop)': ");
					command = Console.ReadLine();
					if (command.ToLower() == "y")
					{
						accToApprove.AccountState = DATA.AccountState.Aproved;
						_context.SaveChanges();
						Console.WriteLine("The account has been approved.");
					}

				}

			} while (command != "stop");
		}

		public void EditUser(string username)
		{
			var user = _context.Users.FirstOrDefault(u => u.Username == username);
			if (user == null)
			{
				throw new ServiceException($"Account with username ({username}) does not exist.");
			}

			Console.Write("Enter edit command: ");
			string[] command = Console.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			while (string.Join(" ", command) != "stop")
			{
				if (string.Join(" ", command).ToLower() == "change password")
				{
					Console.Write("Enter the new password: ");
					string newPassword = Console.ReadLine();
					while (string.IsNullOrEmpty(newPassword))
					{
						Console.WriteLine("Cannot change password to empy.");
						Console.Write("Enter the new password: ");
						newPassword = Console.ReadLine();
					}

					Console.Write("Are you sure 'y' or 'n': ");
					command = Console.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (command[0].ToLower() == "y")
					{
						newPassword = HashPassword(newPassword);
						user.Password = newPassword;
						_context.SaveChanges();
						Console.WriteLine("The password have been changed.");
					}
				}
				else if (string.Join(" ", command).ToLower() == "change email")
				{
					Console.Write("Enter the new email: ");
					string newEmail = Console.ReadLine();
					var regex = new Regex(@"^([0-9a-zA-Z_]([_+-.\w]*[0-9a-zA-Z_])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
					Match match = regex.Match(user.Email);
					while (string.IsNullOrEmpty(newEmail) || !match.Success)
					{

						Console.WriteLine("Wrong email format.");
						Console.Write("Enter the new email: ");
						newEmail = Console.ReadLine();
					}

					Console.Write("Are you sure 'y' or 'n': ");
					command = Console.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (command[0].ToLower() == "y")
					{
						user.Email = newEmail;
						_context.SaveChanges();
						Console.WriteLine($"The email have been changed to {newEmail}.");
					}
				}
				else if (string.Join(" ", command).ToLower() == "change first name")
				{
					Console.Write("Enter the new first name: ");
					string newFirstName = Console.ReadLine();
					while (string.IsNullOrEmpty(newFirstName))
					{

						Console.WriteLine("First name cannot be empty.");
						Console.Write("Enter the new first name: ");
						newFirstName = Console.ReadLine();
					}

					Console.Write("Are you sure 'y' or 'n': ");
					command = Console.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (command[0].ToLower() == "y")
					{
						user.FirstName = newFirstName;
						_context.SaveChanges();
						Console.WriteLine($"The email have been changed to {newFirstName}.");
					}
				}
				else if (string.Join(" ", command).ToLower() == "change last name")
				{
					Console.Write("Enter the new last name: ");
					string newLastName = Console.ReadLine();
					while (string.IsNullOrEmpty(newLastName))
					{

						Console.WriteLine("First name cannot be empty.");
						Console.Write("Enter the new last name: ");
						newLastName = Console.ReadLine();
					}

					Console.Write("Are you sure 'y' or 'n': ");
					command = Console.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (command[0].ToLower() == "y")
					{
						user.LastName = newLastName;
						_context.SaveChanges();
						Console.WriteLine($"The last name have been changed to {newLastName}.");
					}
				}
				else if (string.Join(" ", command).ToLower() == "change role")
				{
					Console.WriteLine("0 - Client");
					Console.WriteLine("1 - Support");
					Console.WriteLine("2 - Administrator");
					Console.Write("Choose role: ");
					DATA.AccountRole role = (DATA.AccountRole)Enum.Parse(typeof(DATA.AccountRole), Console.ReadLine());
					if (role == DATA.AccountRole.Client || role == DATA.AccountRole.Support || role == DATA.AccountRole.Administrator)
					{
						user.Role = role;
						_context.SaveChanges();
						Console.WriteLine($"{username} is now {role}");
					}
					else
					{
						Console.WriteLine("Ivalid role.");
					}


				}
				else
				{
					Console.WriteLine("Wrong edit command. Write 'stop' to stop editing this user");
				}

				Console.Write("Enter edit command: ");
				command = Console.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
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
