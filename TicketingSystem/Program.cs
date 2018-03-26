using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TicketingSystem.Data;
using TicketingSystem.Services;
using TicketingSystem.Services.Impl;

namespace TicketingSystem
{
	class Program
	{
		static void Main(string[] args)
		{
			//DbSeed.Seed(new Data.TicketingSystemDbContext());

			Console.ForegroundColor = ConsoleColor.Green;

			DrawTitle();

			IAccountService accountService = new AccountService();

			LoginResult loginResult = new LoginResult();


			var context = new Data.TicketingSystemDbContext();

			var context = new Data.TicketingSystemDbContext();

			var context = new Data.TicketingSystemDbContext();

			var context = new Data.TicketingSystemDbContext();

			var context = new Data.TicketingSystemDbContext();

			int? userId = null;

<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
			context = new Data.TicketingSystemDbContext();

			userId = -1; 
=======
			context.Database.Migrate();
>>>>>>> parent of 28b8674... Backup commit
=======
			context.Database.Migrate();
>>>>>>> parent of 28b8674... Backup commit
=======
			context.Database.Migrate();
>>>>>>> parent of 28b8674... Backup commit
=======
			context.Database.Migrate();
>>>>>>> parent of 28b8674... Backup commit
=======
			context.Database.Migrate();
>>>>>>> parent of 28b8674... Backup commit

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
				else if (command[0] == "login")
				{
					Console.Write("Username: ");
					string username = Console.ReadLine();

					Console.Write("Password: ");
					string password = Console.ReadLine();

					try
					{
						loginResult = accountService.Login(username, password);
						userId = loginResult.UserId;
						Console.WriteLine("You have been logged in.");
					}
					catch (ServiceException se)
					{
						Console.WriteLine(se.Message);
					}
				}
				else if (string.Join(" ", command) == "create project")
				{
					try
					{
						Console.Write("Project title: ");
						string title = Console.ReadLine();

						Console.Write("Description: ");
						string description = Console.ReadLine();

						ProjectModel projectModel = new ProjectModel(title, description);

						accountService.CreateProject(userId, projectModel);
					}
					catch (ServiceException se)
					{
						Console.WriteLine(se.Message);
					}
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
=======
					
>>>>>>> parent of 28b8674... Backup commit
=======
					
>>>>>>> parent of 28b8674... Backup commit
=======
					
>>>>>>> parent of 28b8674... Backup commit
=======
					
>>>>>>> parent of 28b8674... Backup commit
=======
					
>>>>>>> parent of 28b8674... Backup commit

				}
				else if (command[0] == "logout")
				{
					if (userId == -1)
					{
						Console.WriteLine("You have to be logged in to be logout.");
					}
					else
					{
						userId = -1;
						Console.WriteLine("You have been logged out.");
					}
				}
				else if (command[0] == "help")
				{
					Console.WriteLine("-------------------------------");
					Console.WriteLine("Here are the commands avalable:");
					Console.WriteLine("register, login, logout");
					Console.WriteLine("-------------------------------");
				}

				else if (string.Join(" ", command) == "create ticket")
				{
					Console.Write("Enter project name: ");
					string projectName = Console.ReadLine();

					Console.WriteLine("0 - Bug report ");
					Console.WriteLine("1 - Feature request");
					Console.WriteLine("2 - Assistance request");
					Console.WriteLine("3 - Other");
					Console.Write("Enter type: ");
					TicketType ticketType = (TicketType)Enum.Parse(typeof(TicketType), Console.ReadLine());

					Console.WriteLine("0 - Draft ");
					Console.WriteLine("1 - New");
					Console.WriteLine("2 - Worked on");
					Console.WriteLine("3 - Done");
					Console.Write("Enter state: ");
					StateTicket ticketState = (StateTicket) Enum.Parse(typeof(StateTicket), Console.ReadLine());

					Console.Write("Enter title: ");
					string ticketTitle = Console.ReadLine();

					Console.Write("Enter description: ");
					string ticketDescription = Console.ReadLine();

					Console.Write("Enter file path: ");
					string filePath = Console.ReadLine();
				}
				else if (string.Join(" ",command) == "approve acc")
				{
					if (userId != null)
					{
						if (context.Users.FirstOrDefault(u => u.Id == userId).Role == Role.Administrator)
						{
							accountService.ApproveAccounts();
						}
						else
						{
							Console.WriteLine("You have to be administrator.");
						}
					}
					else
					{
						Console.WriteLine("You are not logged in.");
					}
				}
				else if (string.Join(" ", command) == "edit user")
				{
					if (userId != null)
					{
						if (context.Users.FirstOrDefault(u => u.Id == userId).Role == Role.Administrator)
						{
							try
							{
								Console.Write("Enter username for which account must be edited: ");
								string username = Console.ReadLine();
								accountService.EditUser(username);
							}
							catch (ServiceException se)
							{
								Console.WriteLine(se.Message);
							}
						}
						else
						{
							Console.WriteLine("You have to be administrator.");
						}

					}
					else
					{
						Console.WriteLine("You are not logged in.");
					}
					
				}

			} while (command[0].ToLower() != "exit");
		}

		private static void Register(IAccountService accountService)
		{
			RegisterModel registerModel = CreateRegisterModel();

			try
			{
				accountService.Register(registerModel);
				Console.WriteLine("Your account is being processed");
			}
			catch (ServiceException se)
			{
				Console.WriteLine(se.Message);
			}
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