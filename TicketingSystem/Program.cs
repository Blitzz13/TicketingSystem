using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TicketingSystem.Services;
using TicketingSystem.Services.Impl;
using File = System.IO.File;

namespace TicketingSystem
{
	class Program
	{
		static void Main()
		{
			//DbSeed.Seed(new Data.TicketingSystemDbContext());

			DrawTitle();

			IAccountService accountService = new AccountService();

			IProjectService projectService = new ProjectService();

			ITicketService ticketService = new TicketService();

			//var context = new TicketingSystemDbContext();

			int? userId = null;

			//context.Database.Migrate();

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
					Console.Write("Enter project name: ");
					string projectName = Console.ReadLine();

					Console.Write("Enter ticket title: ");
					string ticketTitle = Console.ReadLine();

					Console.WriteLine("Ticket Types:");
					Console.WriteLine("0 - Bug report");
					Console.WriteLine("1 - Feature request");
					Console.WriteLine("2 - Assistace request");
					Console.WriteLine("3 - Other");
					Console.Write("Enter ticket type: ");
					string ticketType = Console.ReadLine();

					Console.WriteLine("Ticket States:");
					Console.WriteLine("0 - New");
					Console.WriteLine("1 - Draft");
					Console.WriteLine("2 - Worked on");
					Console.WriteLine("3 - Done");
					Console.Write("Enter ticket state: ");
					string ticketState = Console.ReadLine();

					Console.WriteLine("Enter description: ");
					string ticketDescription = Console.ReadLine();

					Console.WriteLine("Enter file path(optional): ");
					TicketModel ticketModel;
					string filePath = Console.ReadLine();

					if (!string.IsNullOrEmpty(filePath))
					{
						byte[] file = File.ReadAllBytes(filePath);
						string fileName = Path.GetFileName(filePath);

						ticketModel = new TicketModel()
						{
							TicketTitle = ticketTitle,
							TicketType = ticketType,
							TicketState = ticketState,
							TicketDescription = ticketDescription,
							FileContent = file,
							FileName = fileName
						};
					}
					else
					{
						ticketModel = new TicketModel()
						{
							TicketTitle = ticketTitle,
							TicketType = ticketType,
							TicketState = ticketState,
							TicketDescription = ticketDescription,
						};
					}

					try
					{
						ticketService.CreateTicket(ticketModel, projectName, userId);
					}
					catch (ServiceException se)
					{
						Console.WriteLine(se.Message);
					}
				}
				else if (string.Join(" ", command) == "approve acc")
				{
					Console.Write("Enter username: ");
					string username = Console.ReadLine();

					try
					{
						accountService.ApproveAccounts(username);
					}
					catch (ServiceException se)
					{
						Console.WriteLine(se.Message);
					}
				}
				else if (string.Join(" ", command) == "edit user")
				{
					Console.Write("Enter username: ");
					string username = Console.ReadLine();

					Console.WriteLine("1 - Change password");
					Console.WriteLine("2 - Change email");
					Console.WriteLine("3 - Change first name");
					Console.WriteLine("4 - Change last name");
					Console.WriteLine("5 - Change role");
					Console.Write("Enter number of command: ");
					int commandNum = int.Parse(Console.ReadLine());

					UserEditModel userEditModel = new UserEditModel()
					{
						Username = username
					};

					switch (commandNum)
					{
						case 1:
							ChangePassword(userEditModel);
							accountService.EditUser(userEditModel, commandNum);
							Console.WriteLine("The password have been changed.");
							break;
						case 2:
							ChangeEmail(userEditModel);
							accountService.EditUser(userEditModel, commandNum);
							Console.WriteLine($"The email have been changed.");
							break;
						case 3:
							ChangeFirstName(userEditModel);
							accountService.EditUser(userEditModel, commandNum);
							Console.WriteLine($"The first name have been changed.");
							break;
						case 4:
							ChangeLastName(userEditModel);
							accountService.EditUser(userEditModel, commandNum);
							Console.WriteLine($"The last name have been changed.");
							break;
						case 5:
							ChangeRole(userEditModel);
							accountService.EditUser(userEditModel, commandNum);
							Console.WriteLine($"The role have been changed.");
							break;
					}
				}
				else if (string.Join(" ", command) == "delete project")
				{
					Console.Write("Enter project name: ");
					string projectName = Console.ReadLine();

					projectService.DeleteProject(projectName);
				}
				else if (string.Join(" ", command) == "delete ticket")
				{
					Console.Write("Enter project name: ");
					string projectName = Console.ReadLine();

					Console.Write("Enter ticket title: ");
					string ticketTitle = Console.ReadLine();

					ticketService.DeleteTicket(projectName, ticketTitle, userId);
				}

			} while (command[0].ToLower() != "exit");
		}

		private static void ChangeRole(UserEditModel userEditModel)
		{
			Console.WriteLine("Client");
			Console.WriteLine("Support");
			Console.WriteLine("Administrator");
			Console.Write("Choose role: ");
			string role = Console.ReadLine();

			userEditModel.Role = role;
		}

		private static void ChangePassword(UserEditModel userEditModel)
		{
			Console.Write("Enter the new password: ");
			string newPassword = Console.ReadLine();
			while (string.IsNullOrEmpty(newPassword))
			{
				Console.WriteLine("Cannot change password to empy.");
				Console.Write("Enter the new password: ");
				newPassword = Console.ReadLine();
			}

			userEditModel.Password = newPassword;
		}

		private static void ChangeEmail(UserEditModel userEditModel)
		{
			Console.Write("Enter the new email: ");
			string newEmail = Console.ReadLine();
			var regex = new Regex(@"^([0-9a-zA-Z_]([_+-.\w]*[0-9a-zA-Z_])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
			Match match = regex.Match(newEmail);
			while (string.IsNullOrEmpty(newEmail) || !match.Success)
			{

				Console.WriteLine("Wrong email format.");
				Console.Write("Enter the new email: ");
				newEmail = Console.ReadLine();
			}

			userEditModel.Email = newEmail;
		}

		private static void ChangeFirstName(UserEditModel userEditModel)
		{
			Console.Write("Enter the new first name: ");
			string newFirstName = Console.ReadLine();
			while (string.IsNullOrEmpty(newFirstName))
			{
				Console.WriteLine("First name cannot be empty.");
				Console.Write("Enter the new first name: ");
				newFirstName = Console.ReadLine();
			}

			userEditModel.FirstName = newFirstName;
		}

		private static void ChangeLastName(UserEditModel userEditModel)
		{
			Console.Write("Enter the new last name: ");
			string newLastName = Console.ReadLine();
			while (string.IsNullOrEmpty(newLastName))
			{
				Console.WriteLine("First name cannot be empty.");
				Console.Write("Enter the new last name: ");
				newLastName = Console.ReadLine();
			}

			userEditModel.LastName = newLastName;
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