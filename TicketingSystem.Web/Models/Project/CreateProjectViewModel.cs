using System.ComponentModel.DataAnnotations;

namespace TicketingSystem.Web.Models.Project
{
	public class CreateProjectViewModel
    {
		public int Id { get; set; }

		[Display(Name = "Project Title")]
		[MinLength(4)]
		[Required]
		public string Name { get; set; }

		[MinLength(5, ErrorMessage = "You should write atleast 5 characters")]
		[Required]
		public string Description { get; set; }

		public int UserId { get; set; }

		public string ErrorMessage { get; set; }
	}
}
