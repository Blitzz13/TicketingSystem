using System;
using System.Linq;
using DATA = TicketingSystem.Data;

namespace TicketingSystem.Services.Impl
{
	public class TicketService : ITicketService
	{
		private readonly DATA.TicketingSystemDbContext _context;

		public TicketService()
		{
			_context = new DATA.TicketingSystemDbContext();
		}

		public void ViewTickets(string projectName, int userId)
		{
			DATA.Project project = _context.Projects.FirstOrDefault(p => p.Name == projectName);

			if (project == null)
			{
				throw new ServiceException("No project found.");
			}

			DATA.User user = _context.Users.FirstOrDefault(u => u.Id == userId);
			if (user.Role == DATA.AccountRole.Administrator || user.Role == DATA.AccountRole.Support)
			{
				foreach (var ticket in project.Tickets.Where(u => u.Submitter.Id == userId))
				{
					Console.WriteLine($"{ticket.Title} - {ticket.Type} - {ticket.State}: ");
					Console.WriteLine($"Submitted on {ticket.SubmissionDate}");
					Console.WriteLine($"Description: {ticket.Description}");
					Console.WriteLine($"Submitted by: {ticket.Submitter}");
					Console.WriteLine($"");
				}
			}
			else
			{
				foreach (var ticket in project.Tickets.Where(u => u.Submitter.Id == userId))
				{
					Console.WriteLine($"{ticket.Title} - {ticket.Type} - {ticket.State}: ");
					Console.WriteLine($"Submitted on {ticket.SubmissionDate}");
					Console.WriteLine($"Description: {ticket.Description}");
				}
			}
			
		}

		public void CreateTicket(TicketModel ticketModel, string projectName, int? userId)
		{
			var project = _context.Projects.FirstOrDefault(p => p.Name == projectName);

			if (project == null)
			{
				throw new ServiceException($"No project with name {projectName}");
			}

			if (userId == null)
			{
				throw new ServiceException("You are not logged in.");
			}

			var user = _context.Users.FirstOrDefault(u => u.Id == userId);

			DATA.TicketType ticketType = (DATA.TicketType)Enum.Parse(typeof(DATA.TicketType), ticketModel.TicketType);
			if (!Enum.IsDefined(typeof(DATA.TicketType), ticketType))
			{
				throw new ServiceException("Invalid Ticket Type.");
			}

			DATA.TicketState ticketState = (DATA.TicketState)Enum.Parse(typeof(DATA.TicketState), ticketModel.TicketState);
			if (!Enum.IsDefined(typeof(DATA.TicketState), ticketState))
			{
				throw new ServiceException("Invalid Ticket State.");
			}

			if (string.IsNullOrEmpty(ticketModel.TicketTitle))
			{
				throw new ServiceException("Title cannot be empty.");
			}

			if (string.IsNullOrEmpty(ticketModel.TicketDescription))
			{
				throw new ServiceException("Description cannot be empty.");
			}

			DATA.Ticket ticket = new DATA.Ticket()
			{
				ProjectId = project.Id,
				Description = ticketModel.TicketDescription,
				SubmissionDate = DateTime.Now,
				Title = ticketModel.TicketTitle,
				Type = ticketType,
				State = ticketState,
				Submitter = user,
			};
			_context.Tickets.Add(ticket);

			DATA.File file = new DATA.File
			{
				Name = ticketModel.FileName,
				Content = ticketModel.FileContent,
				TicketId = ticket.Id,
			};

			_context.Files.Add(file);
			_context.SaveChanges();
		}

		public void DeleteTicket(string projectName, string ticketTitle, int? userId)
		{
			DATA.Project project = _context.Projects.FirstOrDefault(p => p.Name == projectName);

			if (project == null)
			{
				throw new ServiceException($"No project with name {projectName} have been found.");
			}

			DATA.Ticket ticket = _context.Tickets.FirstOrDefault(t => t.Title == ticketTitle && t.Submitter.Id == userId && t.ProjectId == project.Id);
			if (ticket == null)
			{
				throw new ServiceException($"No ticket with ticket title {ticketTitle} was found.");
			}

			_context.Tickets.Remove(ticket);
			_context.SaveChanges();
			Console.WriteLine("The ticket has been removed succsessfully.");
		}
	}
}
