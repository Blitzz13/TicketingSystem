using System.Collections.Generic;

namespace TicketingSystem.Services
{
	public interface ITicketService
	{
		IEnumerable<Ticket> Get(int projectId, int? userId = null);

		int Create(CreateTicketModel model);

		void Delete(int ticketId);
	}
}