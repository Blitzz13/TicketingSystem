using System.Collections.Generic;

namespace TicketingSystem.Services
{
	public interface ITicketService
	{
		int Create(CreateTicketModel model);

		void Edit(UpdateTicketModel model);

		void Delete(int ticketId);

		Ticket GetByTitle(string ticketTitle);

		Ticket GetByProjectId(int projectId);

		Ticket GetByTicketId(int ticketId);

		Ticket GetByProjectIdAndTicketTitle(int projectId, string ticketTitle);

		IEnumerable<Ticket> Get(int projectId, int? userId = null);

		IEnumerable<Ticket> GetAllTicketsForClient(int userId);

		IEnumerable<Ticket> GetAllTicketsToShowForClient(int userId, int page = 1, int PageSize = 5);

		IEnumerable<Ticket> GetAllTicketsForAdminAndSupport();

		IEnumerable<Ticket> GetAllTicketsToShowForAdminAndSupport(int page = 1, int PageSize = 5);

		void ChangeType(UpdateTicketModel model);

		void ChangeState(UpdateTicketModel model);
	}
}