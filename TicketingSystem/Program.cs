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
		private static readonly IMessageService _messageService = new MessageService();
		private static UserIdentity _identity;

		public static bool IsLoggedIn => _identity != null;

		static void Main()
		{
			//DATA.DbSeed.Seed(new Data.TicketingSystemDbContext());

			DrawTitle();

			var context = new DATA.TicketingSystemDbContext();

			context.Database.Migrate();

			while (true)
			{
				Print();
			}
		}

		private static void ViewTickets()
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
				tickets = _ticketService.Get(project.Id, _identity.UserId).ToList();
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

		private static void ViewProject()
		{
			Console.Write("Enter project name: ");
			string projectName = Console.ReadLine();
			Project project = _projectService.GetByName(projectName);
			string username = _userService.GetByUserId(project.UserId).Username;
			Console.WriteLine("------------------------------------------------");
			Console.WriteLine($"Name: {project.Name}");
			Console.WriteLine($"Created by: {username}");
			Console.WriteLine($"Description: {project.Description}");
			Console.WriteLine("------------------------------------------------");
		}

		private static void DeleteProject()
		{
			Console.Write("Enter project name: ");
			string projectName = Console.ReadLine();

			var project = _projectService.GetByName(projectName);
			_projectService.Delete(project.Id);
		}

		private static void DeleteTicket()
		{
			Console.Write("Enter project name: ");
			string projectName = Console.ReadLine();

			Console.Write("Enter ticket title: ");
			string ticketTitle = Console.ReadLine();

			Project project = _projectService.GetByName(projectName);
			Ticket ticket = _ticketService.GetByProjectIdAndTicketTitle(project.Id, ticketTitle);

			_ticketService.Delete(ticket.Id);
		}

		public static void Print()
		{
			int commandNum;
			if (IsLoggedIn)
			{
				if (_identity.IsAdministrator)
				{
					{
						Console.WriteLine("1 - Create Ticket");
						Console.WriteLine("2 - Delete Ticket");
						Console.WriteLine("3 - View Ticket");
						Console.WriteLine("4 - Message Ticket");
						Console.WriteLine("5 - Change Ticket Type");
						Console.WriteLine("6 - Approve User");
						Console.WriteLine("7 - Deny User");
						Console.WriteLine("8 - Edit User");
						Console.WriteLine("9 - Register User");
						Console.WriteLine("10 - Delete User");
						Console.WriteLine("11 - Create Project");
						Console.WriteLine("12 - View Project");
						Console.WriteLine("13 - Delete Project");
						Console.WriteLine("14 - Change Ticket State");
						Console.WriteLine("15 - Logout");
						Console.Write("Enter command number: ");
					}

					commandNum = int.Parse(Console.ReadLine());
					switch (commandNum)
					{
						case 1:
							CreateTicket(_ticketService, _identity.UserId);
							break;
						case 2:
							DeleteTicket();
							break;
						case 3:
							ViewTickets();
							break;
						case 4:
							MessageTicket();
							break;
						case 5:
							ChangeTicketType();
							break;
						case 6:
							ApproveAccount(_userService);
							break;
						case 7:
							DenyAccount(_userService);
							break;
						case 8:
							EditUser();
							break;
						case 9:
							Register(_userService);
							break;
						case 10:
							DeleteUser();
							break;
						case 11:
							CreateProject(_projectService, _identity.UserId);
							break;
						case 12:
							ViewProject();
							break;
						case 13:
							DeleteProject();
							break;
						case 14:
							ChangeTicketState();
							break;
						case 15:
							Logout();
							break;
					}
				}
				else if (_identity.IsSupport)
				{
					{
						Console.WriteLine("1 - Create Ticket");
						Console.WriteLine("2 - Delete Ticket");
						Console.WriteLine("3 - View Tickets");
						Console.WriteLine("4 - Message Ticket");
						Console.WriteLine("5 - Change Ticket Type");
						Console.WriteLine("6 - Change Ticket State");
						Console.WriteLine("7 - Logout");
						Console.Write("Enter command number: ");
					}

					commandNum = int.Parse(Console.ReadLine());
					switch (commandNum)
					{
						case 1:
							CreateTicket(_ticketService, _identity.UserId);
							break;
						case 2:
							DeleteTicket();
							break;
						case 3:
							ViewTickets();
							break;
						case 4:
							MessageTicket();
							break;
						case 5:
							ChangeTicketType();
							break;
						case 6:
							ChangeTicketState();
							break;
						case 7:
							Logout();
							break;
					}
				}
				else if (_identity.IsClient)
				{
					Console.WriteLine("1 - Create Ticket");
					Console.WriteLine("2 - View Tickets");
					Console.WriteLine("3 - Message Ticket");
					Console.WriteLine("4 - Logout");
					Console.Write("Enter command number: ");
					commandNum = int.Parse(Console.ReadLine());

					switch (commandNum)
					{
						case 1:
							CreateTicket(_ticketService, _identity.UserId);
							break;
						case 2:
							ViewTickets();
							break;
						case 3:
							MessageTicket();
							break;
						case 4:
							Logout();
							break;
					}
				}
			}
			else
			{
				Console.WriteLine("1 - Login");
				Console.WriteLine("2 - Register");
				Console.Write("Enter command number: ");
				commandNum = int.Parse(Console.ReadLine());
				switch (commandNum)
				{
					case 1:
						Login();
						break;
					case 2:
						Register(_userService);
						break;
				}
			}


		}

		private static void MessageTicket()
		{
		
			Console.Write("Enter ticket title: ");
			string ticketTitle = Console.ReadLine();
			Ticket ticket = _ticketService.GetByTitle(ticketTitle);
			if (_identity.IsClient && ticket.State != "Done")
			{
				Console.WriteLine("You can't message this ticket");
			}
			else
			{
				CreateMessage(ticket);
			}



		}

		private static void CreateMessage(Ticket ticket)
		{
			Console.Write("Enter your message:");
			string content = Console.ReadLine();
			Console.WriteLine("Choose state");
			Console.WriteLine("Draft");
			Console.WriteLine("Posted");
			Console.Write("Enter state:");
			string state = Console.ReadLine();
			Console.Write("Enter file path(optional): ");
			if (ticket.Title == null)
			{
				Console.WriteLine("Invalid Ticket title!");
			}
			else
			{
				try
				{
					string path = Console.ReadLine();
					byte[] file = File.ReadAllBytes(path);
					string fileName = Path.GetFileName(path);

					var message = new CreateMessageModel
					{
						State = state,
						UserId = _identity.UserId,
						Content = content,
						PublishingDate = DateTime.Now,
						FileContent = file,
						FileName = fileName,
						TicketId = ticket.Id
					};
					_messageService.Create(message);
				}
				catch
				{
					var message = new CreateMessageModel
					{
						State = state,
						UserId = _identity.UserId,
						Content = content,
						PublishingDate = DateTime.Now,
						TicketId = ticket.Id,
					};
					_messageService.Create(message);

				}
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

		private static void Logout()
		{
			_identity = null;
			Console.WriteLine("You have been logged out.");
		}

		private static void CreateProject(IProjectService projectService, int? userId)
		{
			try
			{
				Console.Write("Project title: ");
				string title = Console.ReadLine();

				Console.Write("Description: ");
				string description = Console.ReadLine();

				CreateProjectModel projectModel = new CreateProjectModel(title, description, _identity.UserId);

				projectService.Create(projectModel);
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

			CreateTicketModel ticketModel = GetTicketModel(projectName);

			try
			{
				ticketService.Create(ticketModel);
			}
			catch (ServiceException se)
			{
				Console.WriteLine(se.Message);
			}
		}

		private static void EditUser()
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
					ChangePassword(updateRequest);
					break;
				case 2:
					ChangeEmail(updateRequest);
					Console.WriteLine($"The email have been changed to {updateRequest.Email}.");
					break;
				case 3:
					ChangeFirstName(updateRequest);
					Console.WriteLine($"The first name have been changed.");
					break;
				case 4:
					ChangeLastName(updateRequest);
					Console.WriteLine($"The last name have been changed.");
					break;
				case 5:
					ChangeRole(updateRequest);
					Console.WriteLine($"The role have been changed.");
					break;
			}

			_userService.Update(user.Id, updateRequest);

		}

		private static void DeleteUser()
		{
			Console.Write("Enter username: ");
			string username = Console.ReadLine();

			try
			{
				User user = _userService.GetByUsername(username);
				_userService.Delete(user.Id);
				Console.WriteLine("User account has been deleted.");
			}
			catch (ServiceException se)
			{
				Console.WriteLine(se.Message);
			}
		}

		private static void ApproveAccount(IUserService userService)
		{
			Console.Write("Enter username: ");
			string username = Console.ReadLine();
			User user = _userService.GetByUsername(username);
			try
			{
				userService.Approve(user.Id);
				Console.WriteLine("The account has been approved.");
			}
			catch (ServiceException se)
			{
				Console.WriteLine(se.Message);
			}
		}

		private static void DenyAccount(IUserService userService)
		{
			Console.Write("Enter username: ");
			string username = Console.ReadLine();
			User user = _userService.GetByUsername(username);
			try
			{
				userService.Deny(user.Id);
				Console.WriteLine("The account has been denied.");
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

		private static void ChangePassword(UpdateUserModel updateRequest)
		{
			Console.Write("Enter the new password: ");
			string newPassword = Console.ReadLine();
			while (string.IsNullOrEmpty(newPassword))
			{
				Console.WriteLine("Cannot change password to empy.");
				Console.Write("Enter the new password: ");
				newPassword = Console.ReadLine();
			}

			updateRequest.Password = newPassword;
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

		private static void Register(IUserService registerService)
		{
			CreateUserModel registerModel = CreateRegisterModel();

			try
			{
				registerService.Create(registerModel);
				Console.WriteLine("Your account is being processed");
			}
			catch (ServiceException se)
			{
				Console.WriteLine(se.Message);
			}
		}

		private static CreateTicketModel GetTicketModel(string projectName)
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

			Project project = _projectService.GetByName(projectName);

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
					FileName = fileName,
					ProjectId = project.Id,
					SubmitterId = _identity.UserId
				};
			}

			return new CreateTicketModel()
			{
				TicketTitle = ticketTitle,
				TicketType = ticketType,
				TicketState = ticketState,
				TicketDescription = ticketDescription,
				ProjectId = project.Id,
				SubmitterId = _identity.UserId
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

		private static void ChangeTicketType()
		{
			Console.Write("Enter project name: ");
			string projectName = Console.ReadLine();

			Console.Write("Enter ticket title: ");
			string ticketTitle = Console.ReadLine();

			Project project = _projectService.GetByName(projectName);
			Ticket ticket = _ticketService.GetByProjectIdAndTicketTitle(project.Id, ticketTitle);

			Console.WriteLine("BugReport");
			Console.WriteLine("FeatureRequest");
			Console.WriteLine("AssistanceRequest");
			Console.WriteLine("Other");
			Console.Write("Enter type: ");
			string type = Console.ReadLine();

			UpdateTicketModel model = new UpdateTicketModel
			{
				Id = ticket.Id,
				Type = type
			};

			try
			{
				_ticketService.ChangeType(model);
			}
			catch (ServiceException se)
			{
				Console.WriteLine(se.Message);
			}

		}

		private static void ChangeTicketState()
		{
			Console.Write("Enter project name: ");
			string projectName = Console.ReadLine();

			Console.Write("Enter ticket title: ");
			string ticketTitle = Console.ReadLine();

			Project project = _projectService.GetByName(projectName);
			Ticket ticket = _ticketService.GetByProjectIdAndTicketTitle(project.Id, ticketTitle);

			Console.WriteLine("Draft");
			Console.WriteLine("New");
			Console.WriteLine("WorkedOn");
			Console.WriteLine("Done");
			Console.Write("Enter state: ");
			string state = Console.ReadLine();

			UpdateTicketModel model = new UpdateTicketModel
			{
				Id = ticket.Id,
				State = state
			};

			try
			{
				_ticketService.ChangeState(model);
			}
			catch (ServiceException se)
			{
				Console.WriteLine(se.Message);
			}

		}
	}
}