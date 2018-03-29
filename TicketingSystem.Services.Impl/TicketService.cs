using System;
using System.Collections.Generic;
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

		public IEnumerable<Ticket> Get(int projectId, int? userId = null)
		{
			IQueryable<DATA.Ticket> tickets = _context.Tickets.Where(t => t.ProjectId == projectId);
			if (userId != null)
			{
				_context.Tickets.Where(t => t.Submitter.Id == userId.Value);
			}

			return tickets.ToList().Select(CreateTicket);
		}

		public int Create(CreateTicketModel model)
		{
			DATA.TicketType ticketType = (DATA.TicketType)Enum.Parse(typeof(DATA.TicketType), model.TicketType);
			if (!Enum.IsDefined(typeof(DATA.TicketType), ticketType))
			{
				throw new ServiceException("Invalid Ticket Type.");
			}

			DATA.TicketState ticketState = (DATA.TicketState)Enum.Parse(typeof(DATA.TicketState), model.TicketState);
			if (!Enum.IsDefined(typeof(DATA.TicketState), ticketState))
			{
				throw new ServiceException("Invalid Ticket State.");
			}

			DATA.Ticket ticket = new DATA.Ticket()
			{
				ProjectId = model.ProjectId,
				Description = model.TicketDescription,
				SubmissionDate = DateTime.Now,
				Title = model.TicketTitle,
				Type = ticketType,
				State = ticketState,
				SubmitterId = model.SubmitterId,
			};

			_context.Tickets.Add(ticket);

			DATA.File file = new DATA.File
			{
				Name = model.FileName,
				Content = model.FileContent,
				TicketId = ticket.Id,
			};

			_context.Files.Add(file);
			_context.SaveChanges();

			return ticket.Id;
		}

		public void Delete(int ticketId)
		{
			DATA.Ticket ticket = _context.Tickets.First(p => p.Id == ticketId);

			_context.Tickets.Remove(ticket);
			_context.SaveChanges();
		}

		public static Ticket CreateTicket(DATA.Ticket ticket)
		{
			return new Ticket
			{
				Id = ticket.Id,
				Submitter = ticket.Submitter.FirstName,
				ProjectId = ticket.ProjectId,

			};
		}
	}
}
