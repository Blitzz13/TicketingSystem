namespace TickektingSystem.Services
{
	public class RegisterResult
	{

		public bool IsSuccessful { get; set; }

		public bool InvalidUsername { get; set; }

		public bool UsernameTaken { get; set; }

		public bool IvalidUsernameSize { get; set; }

		public bool IncorrectEmailFormat { get; set; }

		public bool InvalidFirstName { get; set; }

		public bool InvalidLastName { get; set; }

		public bool InvalidEmail { get; set; }

		public bool InvalidPassword { get; set; }
	}
}
