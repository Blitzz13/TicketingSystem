using System.Collections.Generic;

namespace TicketingSystem.Services
{
	public interface ITicketService
	{
		ICollection<Data.Ticket> ViewTickets(string project, int? userId);

		void CreateTicket(TicketModel ticketModel, string projectName, int? userId);

		void DeleteTicket(string projectName, string ticketTitle, int? userId);
	}
}
