using System;

namespace TicketingSystem.Services
{
	public class Ticket
	{
		public int Id { get; set; }

		public DateTime SubmissionDate { get; set; }

		public int SubmitterId { get; set; }

		public string Submitter { get; set; }

		public string Type { get; set; }

		public string State { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public string FileCount { get; set; }

		public int ProjectId { get; set; }

		public string[] Messages { get; set; }
	}
}
