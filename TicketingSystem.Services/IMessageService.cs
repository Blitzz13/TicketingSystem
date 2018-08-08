using System.Collections.Generic;

namespace TicketingSystem.Services
{
	public interface IMessageService
	{
		IEnumerable<Message> Get(int ticketId);

		int Create(CreateMessageModel model);
	}
}
