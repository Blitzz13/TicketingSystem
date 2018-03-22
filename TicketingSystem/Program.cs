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

			DrawTitle();

			IAccountService accountService = new AccountService();

			IProjectService projectService = new ProjectService();

			var context = new Data.TicketingSystemDbContext();

			int? userId = null;

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
				else if (command[0] == "login")
				{
					Console.Write("Username: ");
					string username = Console.ReadLine();

					Console.Write("Password: ");
					string password = Console.ReadLine();

					try
					{
						var loginResult = accountService.Login(username, password);
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
					if (userId != null)
					{
						if (context.Users.FirstOrDefault(u => u.Id == userId).Role == Role.Administrator)
						{
							try
							{
								Console.Write("Project title: ");
								string title = Console.ReadLine();

								Console.Write("Description: ");
								string description = Console.ReadLine();

								ProjectModel projectModel = new ProjectModel(title, description);

								projectService.CreateProject(userId, projectModel);
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
				else if (command[0] == "logout")
				{
					if (userId == null)
					{
						Console.WriteLine("You have to be logged in to be logout.");
					}
					else
					{
						userId = null;
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

				}
				else if (string.Join(" ", command) == "change acc role")
				{
					if (userId != null)
					{
						if (context.Users.FirstOrDefault(u => u.Id == userId).Role == Role.Administrator)
						{
							try
							{
								Console.Write("Targeted username: ");
								string username = Console.ReadLine();

								Console.WriteLine("Choose role: ");
								Console.WriteLine("0 - Client");
								Console.WriteLine("1 - Support");
								Console.WriteLine("2 - Administrator");

								Role role = (Role)Enum.Parse(typeof(Role), Console.ReadLine());
								accountService.ChangeRole(username, userId, role);
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
							Console.Write("Enter username for which account must be edited: ");
							string username = Console.ReadLine();

							accountService.EditUser(username);
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