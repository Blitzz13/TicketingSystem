using System.ComponentModel.DataAnnotations;

namespace TicketingSystem.Web.Models.Account
{
	public class RegisterViewModel
	{
		[MinLength(5, ErrorMessage = "First name should be atleast 5 chars")]
		public string Username { get; set; }

		[MinLength(2,ErrorMessage = "First name should be atleast 2 chars")]
		[Display(Name = "First Name")]
		public string FirstName { get; set; }

		[MinLength(2, ErrorMessage = "Last name should be atleast 2 chars")]
		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		[MinLength(5, ErrorMessage = "First name should be atleast 5 chars")]
		public string Password { get; set; }

		[EmailAddress]
		[Display(Name = "E-main")]
		public string Email { get; set; }

		[Display(Name = "Account State")]
		public string AccountState { get; set; }

		public string ErrorMessage { get; set; }
	}
}
