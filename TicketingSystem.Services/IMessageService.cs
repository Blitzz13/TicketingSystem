using System.Collections.Generic;

namespace TicketingSystem.Services
{
	public interface IMessageService
	{
		IEnumerable<Message> Get(int ticketId);

		Message GetById(int messageId);

		int Create(CreateMessageModel model);

		void Delete(int id);

		void Edit(EditMessageModel model);

		void ChangeStateToPost(int id);
	}
}
