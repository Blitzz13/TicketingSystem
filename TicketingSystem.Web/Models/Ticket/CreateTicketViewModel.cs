using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TicketingSystem.Web.Models.Ticket
{
	public class CreateTicketViewModel
	{
		public List<string> ProjectNames { get; set; }

		public string ProjectName { get; set; }

		[Display(Name = "Ticket title")]
		public string TicketTitle { get; set; }

		[Display(Name = "Ticket types")]
		public string TicketTypes { get; set; }

		[Display(Name = "Ticket states")]
		public string TicketStates { get; set; }

		public string Description { get; set; }

		public byte[] FileContent { get; set; }

		public string FileName { get; set; }

		public int ProjectId { get; set; }

		public int SubmitterId { get; set; }
	}
}
