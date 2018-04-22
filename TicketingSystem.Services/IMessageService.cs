using System.Collections.Generic;

namespace TicketingSystem.Services
{
	public interface IMessageService
	{
		IEnumerable<Message> Get(int projectId, int ticketId, int? userId = null);

		int Create(CreateMessageModel model);
	}
}
