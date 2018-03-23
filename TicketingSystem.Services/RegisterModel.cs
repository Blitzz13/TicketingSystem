namespace TicketingSystem.Services
{
	public class RegisterModel
	{

		public RegisterModel(string userName, string passowrd, string email, string firstName, string lastName)
		{
			UserName = userName;
			Passowrd = passowrd;
			Email = email;
			FirstName = firstName;
			LastName = lastName;
		}

		public string UserName { get; set; }

		public string Passowrd { get; set; }

		public string Email { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }
	}
}
