using System;

namespace TicketingSystem.Web.Models.Ticket
{
	public class OverviewTicketViewModel
	{
		public string Title { get; set; }

		public string SumitterName { get; set; }

		public string ProjectName { get; set; }

		public DateTime SubmissionDate { get; set; }

		public string TicketState { get; set; }

		public int MessagesCount { get; set; }
	}
}
