using System.ComponentModel.DataAnnotations;

namespace TicketingSystem.Web.Models.Account
{
	public class LoginViewModel
	{
		[Required]
		public string Username { get; set; }

		[Required]
		public string Password { get; set; }

		public string ErrorMessage { get; set; }
	}
}
