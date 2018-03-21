using Microsoft.EntityFrameworkCore;
using System;
using TicketingSystem.Services;
using TicketingSystem.Services.Impl;

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

			var context = new Data.TicketingSystemDbContext();

			context.Database.Migrate();

			string[] command;

			do
			{
				Console.Write("Please enter a command: ");
				command = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);

				//make all letters low
				for (int i = 0; i < command.Length; i++)
				{
					command[i] = command[i].ToLower();
				}

				if (command[0] == "register")
				{
					Register(accountService);

				}
				//else if (command[0] == "login")
				//{
				//	Console.Write("Username: ");
				//	string username = Console.ReadLine();

				//	Console.Write("Password: ");
				//	string password = Console.ReadLine();

				//	loginResult = accountService.Login(username, password);
				//	if (user.AccountState == AccountState.Pending)
				//	{
				//		Console.WriteLine("Your account haven't been activated yet! Try again later.");
				//	}
				//	else if (user.AccountState == AccountState.Aproved)
				//	{
				//		Console.WriteLine("You have been logged in");
				//	}



				//}
				//else if (string.Join(" ", command) == "create project")
				//{
				//	if (user != null)
				//	{
				//		if (user.Role == Role.Administrator)
				//		{
							
				//		}
				//	}
				//}

			} while (command[0].ToLower() != "exit");
		}

		private static void Register(IAccountService accountService)
		{
			RegisterModel registerModel = CreateRegisterModel();

			try
			{
				accountService.Register(registerModel);
				Console.WriteLine("");
			}
			catch (ServiceException e)
			{
				Console.WriteLine(e.Message);
			}
		}

		//private static User VerifyUserPasswordAndGetUser(TicketingSystemDbContext context, string username, string password, LoginResult loginResult)
		//{
		//	string savedPasswordHash = context.Users.FirstOrDefault(u => u.Username == username).Password;
		//	byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
		//	byte[] salt = new byte[16];
		//	Array.Copy(hashBytes, 0, salt, 0, 16);
		//	var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
		//	byte[] hash = pbkdf2.GetBytes(20);

		//	for (int i = 0; i < 20; i++)
		//	{
		//		if (hashBytes[i + 16] != hash[i])
		//		{
		//			Console.WriteLine("Wrong password!");
		//			loginResult.WrongPassword = true;
		//			break;
		//		}
		//	}

		//	if (!loginResult.WrongPassword)
		//	{
		//		loginResult.IsSuccsessful = true;
		//		return context.Users.FirstOrDefault(u => u.Username == username);
		//	}

		//	return null;
		//}

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