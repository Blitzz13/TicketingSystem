using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DATA = TicketingSystem.Data;
using TicketingSystem.Services;
using TicketingSystem.Services.Impl;
using File = System.IO.File;

namespace TicketingSystem
{
	class Program
	{
		private static readonly IUserService _userService = new UserService();
		private static readonly IProjectService _projectService = new ProjectService();
		private static readonly ITicketService _ticketService = new TicketService();
		private static UserIdentity _identity;

		public static bool IsLoggedIn => _identity != null;

		static void Main()
		{
			//DbSeed.Seed(new Data.TicketingSystemDbContext());

			DrawTitle();



			var context = new DATA.TicketingSystemDbContext();

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
					Register(_userService);
				}
				else if (command[0] == "login")
				{
					Login();
				}
				else if (string.Join(" ", command) == "create project")
				{
					CreateProject(_projectService, userId);
				}
				else if (command[0] == "logout")
				{
					Logout();
				}
				else if (string.Join(" ", command) == "create ticket")
				{
					CreateTicket(_ticketService, userId);
				}
				else if (string.Join(" ", command) == "approve acc")
				{
					ApproveAccount(_userService);
				}
				else if (string.Join(" ", command) == "edit user")
				{
					EditUser(_userService);
				}
				else if (string.Join(" ", command) == "delete project")
				{
					Console.Write("Enter project name: ");
					string projectName = Console.ReadLine();

					var project = _projectService.GetByName(projectName);
					_projectService.Delete(project.Id);
				}
				else if (string.Join(" ", command) == "delete ticket")
				{
					Console.Write("Enter project name: ");
					string projectName = Console.ReadLine();

					Console.Write("Enter ticket title: ");
					string ticketTitle = Console.ReadLine();

					_ticketService.Delete(projectName, ticketTitle, userId);
				}
				else if (string.Join(" ", command) == "view tickets")
				{
					Console.Write("Enter project name: ");
					string projectName = Console.ReadLine();
					List<Ticket> tickets;
					Project project = _projectService.GetByName(projectName);

					if (_identity.IsAdministrator || _identity.IsSupport)
					{
						tickets = _ticketService.Get(project.Id).ToList();
					}
					else
					{
						tickets = _ticketService.Get(project.Id,_identity.UserId).ToList();
					}
					

					Console.WriteLine("----------------------------------------");
					foreach (var ticket in tickets)
					{
						Console.WriteLine($"Title: {ticket.Title}");
						Console.WriteLine($"Submited by: {ticket.Submitter}");
						Console.WriteLine($"Number of files: {ticket.FileCount}");
						Console.WriteLine($"State: {ticket.State}");
						Console.WriteLine($"Submited on: {ticket.SubmissionDate}");
						Console.WriteLine($"Description: {ticket.Description}");
						Console.WriteLine("----------------------------------------");
					}
				}

			} while (command[0].ToLower() != "exit");
		}

		public static void Print()
		{
			if (IsLoggedIn)
			{
				if (_identity.IsAdministrator)
				{
					
				}
				else if (_identity.IsClient)
				{

				}
				else if (_identity.IsSupport)
				{

				}

				
			}
			else
			{
				
			}


		}

		private static void Login()
		{
			Console.Write("Username: ");
			string username = Console.ReadLine();

			Console.Write("Password: ");
			string password = Console.ReadLine();

			try
			{
				LoginResult loginResult = _userService.Login(username, password);
				Console.WriteLine("You have been logged in.");

				_identity = new UserIdentity
				{
					UserId = loginResult.UserId,
					IsAdministrator = loginResult.IsAdministrator,
					IsClient = loginResult.IsClient,
					IsSupport = loginResult.IsSupport
				};
			}
			catch (ServiceException se)
			{
				Console.WriteLine(se.Message);
			}
		}

		public static void Logout()
		{
			_identity = null;
		}

		private static void CreateProject(IProjectService projectService, int? userId)
		{
			try
			{
				Console.Write("Project title: ");
				string title = Console.ReadLine();

				Console.Write("Description: ");
				string description = Console.ReadLine();

				CreateProjectModel projectModel = new CreateProjectModel(title, description);

				projectService.Create(userId, projectModel);
			}
			catch (ServiceException se)
			{
				Console.WriteLine(se.Message);
			}
		}

		private static void CreateTicket(ITicketService ticketService, int? userId)
		{
			Console.Write("Enter project name: ");
			string projectName = Console.ReadLine();

			CreateTicketModel ticketModel = GetTicketModel();

			try
			{
				ticketService.Create(ticketModel, projectName, userId);
			}
			catch (ServiceException se)
			{
				Console.WriteLine(se.Message);
			}
		}

		private static void EditUser(IUserService accountService)
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

			User user = _userService.GetByUsername(username);
			var updateRequest = new UpdateUserModel();

			switch (commandNum)
			{
				case 1:
					Console.Write("Enter new password: ");
					updateRequest.Password = Console.ReadLine();
					break;
				case 2:
					Console.Write("Enter new email: ");
					updateRequest.Email = Console.ReadLine();
					Console.WriteLine($"The email have been changed.");
					break;
				case 3:
					Console.Write("Enter new first name: ");
					updateRequest.FirstName = Console.ReadLine();
					Console.WriteLine($"The first name have been changed.");
					break;
				case 4:
					Console.Write("Enter new last name: ");
					updateRequest.LastName = Console.ReadLine();
					Console.WriteLine($"The last name have been changed.");
					break;
				case 5:
					Console.Write("Enter new role: ");
					updateRequest.Role = Console.ReadLine();
					Console.WriteLine($"The role have been changed.");
					break;
			}
			
			_userService.Update(user.Id, updateRequest);

		}

		private static void ApproveAccount(IUserService accountService)
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

		private static void ChangeRole(UpdateUserModel userEditModel)
		{
			Console.WriteLine("Client");
			Console.WriteLine("Support");
			Console.WriteLine("Administrator");
			Console.Write("Choose role: ");
			string role = Console.ReadLine();

			userEditModel.Role = role;
		}

		private static void ChangePassword(UpdateUserModel userEditModel)
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

		private static void ChangeEmail(UpdateUserModel userEditModel)
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

		private static void ChangeFirstName(UpdateUserModel userEditModel)
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

		private static void ChangeLastName(UpdateUserModel userEditModel)
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

		private static void Register(IUserService accountService)
		{
			CreateUserModel registerModel = CreateRegisterModel();

			try
			{
				accountService.Create(registerModel);
				Console.WriteLine("Your account is being processed");
			}
			catch (ServiceException se)
			{
				Console.WriteLine(se.Message);
			}
		}

		private static CreateTicketModel GetTicketModel()
		{

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
			string filePath = Console.ReadLine();

			if (!string.IsNullOrEmpty(filePath))
			{
				byte[] file = File.ReadAllBytes(filePath);
				string fileName = Path.GetFileName(filePath);

				return new CreateTicketModel()
				{
					TicketTitle = ticketTitle,
					TicketType = ticketType,
					TicketState = ticketState,
					TicketDescription = ticketDescription,
					FileContent = file,
					FileName = fileName
				};
			}

			return new CreateTicketModel()
			{
				TicketTitle = ticketTitle,
				TicketType = ticketType,
				TicketState = ticketState,
				TicketDescription = ticketDescription,
			};

		}

		private static void DrawTitle()
		{
			Console.WriteLine("		--------------------------------------------------------");
			Console.WriteLine("		------------Welcome to your Ticketing System------------");
			Console.WriteLine("		--------------------------------------------------------");
		}

		private static CreateUserModel CreateRegisterModel()
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

			return new CreateUserModel(username, password, email, firstName, lastName);
		}
	}
}