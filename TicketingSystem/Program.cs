using Microsoft.EntityFrameworkCore;
using System;
using TickektingSystem.Data;
using TickektingSystem.Services;
using TickektingSystem.Services.Impl;

namespace TicketingSystem
{
	class Program
	{
		static void Main(string[] args)
		{
			//DbSeed.Seed(new Data.TicketingSystemDbContext());

			DrawTitle();

			IAccountService accountService = new AccountService();

			var context = new TicketingSystemDbContext();

			context.Database.Migrate();

			string[] command;

			do
			{
				Console.Write("Please enter a command: ");
				command = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);

				if (command[0].ToLower() == "register")
				{
					RegisterModel registerModel = CreateRegisterModel();

					RegisterResult registerResult = accountService.Register(registerModel);
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
				}

			} while (command[0].ToLower() != "exit");
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
