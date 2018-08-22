using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using DATA = TicketingSystem.Data;

namespace TicketingSystem.Services.Impl
{
	public class UserService : IUserService
	{
		private readonly DATA.TicketingSystemDbContext _context;

		public UserService()
		{
			_context = new DATA.TicketingSystemDbContext();
		}

		#region  AccountService members

		public LoginResult Login(string userName, string password)
		{
			DATA.User user = _context.Users.FirstOrDefault(u => u.Username == userName);
			if (user == null)
			{
				throw new ServiceException("Wrong username or passowrd");
			}

			byte[] hashBytes, hash;
			UnhashPassword(password, user, out hashBytes, out hash);
			ValidatePassword(hashBytes, hash);

			if (user.AccountState == DATA.AccountState.Pending)
			{
				throw new ServiceException("Your account is not approved yet. Please try again later.");
			}

			if (user.AccountState == DATA.AccountState.Denied)
			{
				throw new ServiceException("Your account is denied.");
			}

			var result = new LoginResult
			{
				UserId = user.Id,
				Username = user.Username
			};

			switch (user.Role)
			{
				case DATA.AccountRole.Administrator:
					result.IsAdministrator = true;
					break;
				case DATA.AccountRole.Support:
					result.IsSupport = true;
					break;
				case DATA.AccountRole.Client:
					result.IsClient = true;
					break;
			}

			return result;
		}

		public void Create(CreateUserModel model)
		{
			if (string.IsNullOrEmpty(model.FirstName))
			{
				throw new ServiceException("Your first name cannot be empty.");
			}

			if (string.IsNullOrEmpty(model.LastName))
			{
				throw new ServiceException("Your last name cannot be empty.");
			}

			if (string.IsNullOrEmpty(model.UserName))
			{
				throw new ServiceException("Your username cannot be empty.");
			}

			if (_context.Users.Any(u => u.Username == model.UserName))
			{
				throw new ServiceException("The username you have chosen already exists.");
			}

			if (string.IsNullOrEmpty(model.Email))
			{
				throw new ServiceException("The email cannot be empty");
			}

			var regex = new Regex(@"^([0-9a-zA-Z_]([_+-.\w]*[0-9a-zA-Z_])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
			Match match = regex.Match(model.Email);
			if (!match.Success)
			{
				throw new ServiceException("The email you enterted is in incorrect format");
			}

			if (model.UserName.Length < 3)
			{
				throw new ServiceException("The username should be more than 2 characters");
			}

			string password = HashPassword(model.Passowrd);

			DATA.User user = new DATA.User();

			if (model.AccountState != AccountState.Pending)
			{
				user = new DATA.User
				{
					Username = model.UserName,
					Password = password,
					Email = model.Email,
					FirstName = model.FirstName,
					LastName = model.LastName
				};

				if (model.AccountState == AccountState.Approved)
				{
					user.AccountState = DATA.AccountState.Approved;
				}
				else
				{
					user.AccountState = DATA.AccountState.Denied;
				}
				
				_context.Add(user);
				_context.SaveChanges();
			}
			else
			{
				user = new DATA.User
				{
					Username = model.UserName,
					Password = password,
					Email = model.Email,
					FirstName = model.FirstName,
					LastName = model.LastName,
					AccountState = DATA.AccountState.Pending
				};

				_context.Add(user);
				_context.SaveChanges();
			}

		}

		public void Approve(int userId)
		{
			var accToApprove = _context.Users.FirstOrDefault(u => u.Id == userId);

			if (accToApprove == null)
			{
				throw new ServiceException("No account found with that name.");
			}

			if (accToApprove.AccountState == DATA.AccountState.Approved)
			{
				throw new ServiceException("This account has already been approved.");
			}

			accToApprove.AccountState = DATA.AccountState.Approved;
			_context.SaveChanges();
		}

		public void Deny(int userId)
		{
			var accToDeny = _context.Users.FirstOrDefault(u => u.Id == userId);

			if (accToDeny == null)
			{
				throw new ServiceException("No account found with that name.");
			}

			if (accToDeny.AccountState == DATA.AccountState.Denied)
			{
				throw new ServiceException("This account has already been approved.");
			}

			accToDeny.AccountState = DATA.AccountState.Denied;
			_context.SaveChanges();
		}

		public void Update(int userId, UpdateUserModel model)
		{
			var user = _context.Users.FirstOrDefault(u => u.Id == userId);
			if (user == null)
			{
				throw new ServiceException($"Account with username ({model.Username}) does not exist.");
			}

			if (!string.IsNullOrEmpty(model.FirstName))
			{
				user.FirstName = model.FirstName;
			}

			if (!string.IsNullOrEmpty(model.LastName))
			{
				user.LastName = model.LastName;
			}

			if (!string.IsNullOrEmpty(model.Role))
			{
				DATA.AccountRole role = (DATA.AccountRole)Enum.Parse(typeof(DATA.AccountRole), model.Role);
				user.Role = role;
			}

			if (!string.IsNullOrEmpty(model.Password))
			{
				model.Password = HashPassword(model.Password);
				user.Password = model.Password;
			}

			if (!string.IsNullOrEmpty(model.Email))
			{
				user.Email = model.Email;
			}

			_context.SaveChanges();
		}

		public void Delete(int userId)
		{
			DATA.User user = _context.Users.First(u => u.Id == userId);

			_context.Users.Remove(user);
			_context.SaveChanges();
		}

		public IEnumerable<User> GetUnApprovedUsers(int page = 1, int PageSize = 5)
		{
			List<User> unApprovedUsers =
				_context
				.Users
				.Where(u => u.AccountState.ToString() == "Pending")
				.Select(CreateUser)
				.OrderByDescending(p => p.Id)
				.Skip((page - 1) * PageSize)
				.Take(PageSize)
				.ToList();

			return unApprovedUsers;
		}

		public User GetByUsername(string userName)
		{
			DATA.User user = _context.Users.FirstOrDefault(u => u.Username == userName);
			if (user == null)
			{
				throw new ServiceException("User not found.");
			}

			return CreateUser(user);
		}

		public User GetByUserId(int userId)
		{
			DATA.User user = _context.Users.FirstOrDefault(u => u.Id == userId);

			return CreateUser(user);
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
					throw new ServiceException("Wrong username or passowrd");
				}
			}
		}

		private static User CreateUser(DATA.User user)
		{
			return new User()
			{
				Id = user.Id,
				Username = user.Username,
				Email = user.Email,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Role = user.Role.ToString()
			};
		}

		public IEnumerable<User> GetAllUnApprovedUsers()
		{
			List<User> allUnApprovedUsers =
				_context
				.Users
				.OrderByDescending(p => p.Id)
				.Where(u => u.AccountState.ToString() == "Pending")
				.Select(CreateUser)
				.ToList();

			return allUnApprovedUsers;
		}

		public IEnumerable<User> GetAllApprovedUsers(int page = 1, int PageSize = 5)
		{
			List<User> approvedUsers =
				_context
				.Users
				.OrderByDescending(p => p.Id)
				.Where(u => u.AccountState.ToString() != "Pending")
				.Select(CreateUser)
				.ToList();

			return approvedUsers;
		}
	}
}
