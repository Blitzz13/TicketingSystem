using System;
using System.Collections.Generic;
using System.Linq;
using DATA = TicketingSystem.Data;
namespace TicketingSystem.Services.Impl
{
	public class MessageService : IMessageService
	{
		private readonly DATA.TicketingSystemDbContext _context;

		public MessageService()
		{
			_context = new DATA.TicketingSystemDbContext();
		}

		public IEnumerable<Message> Get(int ticketId)
		{
			IQueryable<DATA.Message> messages = _context.Messages.Where(m => m.TicketId == ticketId);

			return messages.ToList().Select(CreateMessage);
		}

		public int Create(CreateMessageModel model)
		{
			DATA.StateMessage messageState = (DATA.StateMessage)Enum.Parse(typeof(DATA.StateMessage), model.State);
			if (!Enum.IsDefined(typeof(DATA.StateMessage), messageState))
			{
				throw new ServiceException("Invalid message state.");
			}

			var message = new DATA.Message
			{
				UserId = model.UserId,
				Content = model.Content,
				PublishingDate = model.PublishingDate,
				State = messageState,
				TicketId = model.TicketId
			};

			_context.Add(message);

			if (!string.IsNullOrEmpty(model.FileName))
			{
				DATA.File file = new DATA.File
				{
					Name = model.FileName,
					Content = model.FileContent,
					MessageId = message.Id
				};

				_context.Files.Add(file);
			}

			_context.Add(message);

			_context.SaveChanges();

			return message.Id;
		}

		public static Message CreateMessage(DATA.Message message)
		{
			UserService userService = new UserService();
			return new Message
			{
				Id = message.Id,
				UserId = message.UserId,
				Username = userService.GetByUserId(message.UserId).Username,
				State =  message.State.ToString(),
				Content = message.Content,
				PublishingDate = message.PublishingDate
			};
		}
	}
}
