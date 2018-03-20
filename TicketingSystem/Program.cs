using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using TickektingSystem.Data;
using TickektingSystem.Data.Enums;
using TickektingSystem.Services;
using TickektingSystem.Services.Impl;
using TicketingSystem.Data.Models;

namespace TicketingSystem
{
	class Program
	{
		static void Main(string[] args)
		{
			//DbSeed.Seed(new Data.TicketingSystemDbContext());

			DrawTitle();

			IAccountService accountService = new AccountService();

			LoginResult loginResult = new LoginResult();

			var context = new TicketingSystemDbContext();

			var user = new User();

			context.Database.Migrate();

			string[] command;

			do
			{
				Console.Write("Please enter a command: ");
				command = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);

				if (command[0].ToLower() == "register")
				{
					Register(accountService);

				}

				if (command[0].ToLower() == "login")
				{
					Console.Write("Username: ");
					string username = Console.ReadLine();

					Console.Write("Password: ");
					string password = Console.ReadLine();

					user = VerifyUserPasswordAndGetUser(context, username, password, loginResult);
					if (user != null)
					{
						if (user.AccountState == AccountState.Pending)
						{
							Console.WriteLine("Your account haven't been activated yet! Try again later.");
							user = null;
						}
						else if (user.AccountState == AccountState.Aproved && loginResult.IsSuccsessful)
						{
							Console.WriteLine("You have been logged in");
						}
					}

				}

			} while (command[0].ToLower() != "exit");
		}

		private static void Register(IAccountService accountService)
		{
			RegisterModel registerModel = CreateRegisterModel();

			RegisterResult registerResult = accountService.Register(registerModel);

			GetRegisterResult(registerResult);
		}

		private static void GetRegisterResult(RegisterResult registerResult)
		{
			if (registerResult.IsSuccessful)
			{
				Console.WriteLine("Your account is now being processed.");
			}
			else if (registerResult.InvalidUsername)
			{
				Console.WriteLine("Your username cannot be empty.");
			}
			else if (registerResult.UsernameTaken)
			{
				Console.WriteLine("The username you have chosen already exists.");
			}
			else if (registerResult.InvalidPassword)
			{
				Console.WriteLine("Your password cannot be empty.");
			}
			else if (registerResult.InvalidLastName)
			{
				Console.WriteLine("Your last name cannot be empty.");
			}
			else if (registerResult.InvalidFirstName)
			{
				Console.WriteLine("Your first name cannot be empty.");
			}
			else if (registerResult.InvalidEmail)
			{
				Console.WriteLine("Your email must be valid");
			}
			else if (registerResult.IncorrectEmailFormat)
			{
				Console.WriteLine("The email you enterted is in incorrect format");
			}
			else if (registerResult.IvalidUsernameSize)
			{
				Console.WriteLine("The username should be more than 2 characters");
			}
		}

		private static User VerifyUserPasswordAndGetUser(TicketingSystemDbContext context, string username, string password, LoginResult loginResult)
		{
			string savedPasswordHash = context.Users.FirstOrDefault(u => u.Username == username).Password;
			byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
			byte[] salt = new byte[16];
			Array.Copy(hashBytes, 0, salt, 0, 16);
			var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
			byte[] hash = pbkdf2.GetBytes(20);

			for (int i = 0; i < 20; i++)
			{
				if (hashBytes[i + 16] != hash[i])
				{
					Console.WriteLine("Wrong password!");
					loginResult.WrongPassword = true;
					break;
				}
			}

			if (!loginResult.WrongPassword)
			{
				loginResult.IsSuccsessful = true;
				return context.Users.FirstOrDefault(u => u.Username == username);
			}

			return null;
		}

		private static void DrawTitle()
		{
			Console.WriteLine("		--------------------------------------------------------");
			Console.WriteLine("		------------Welcome to your Ticketing System------------");
			Console.WriteLine("		--------------------------------------------------------");
		}

		private static RegisterModel CreateRegisterModel()
		{
			Console.Write("Username: ");
			string username = Console.ReadLine();

			Console.Write("Password: ");
			string password = Console.ReadLine();

			Console.Write("Email: ");
			string email = Console.ReadLine();

			Console.Write("First Name: ");
			string firstName = Console.ReadLine();

			Console.Write("Last Name: ");
			string lastName = Console.ReadLine();

			return new RegisterModel(username, password, email, firstName, lastName);
		}
	}
}