namespace TicketingSystem.Services
{
	public interface ITicketService
	{
		void ViewTickets(string project, int userId);

		void CreateTicket(TicketModel ticketModel, string projectName, int? userId);

		void DeleteTicket(string projectName, string ticketTitle, int userId);
	}
}
