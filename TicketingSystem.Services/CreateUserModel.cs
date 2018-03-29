using TicketingSystem.Data;

namespace TicketingSystem.Services
{
	public class CreateUserModel
	{

		public CreateUserModel(string userName, string passowrd, string email, string firstName, string lastName)
		{
			UserName = userName;
			Passowrd = passowrd;
			Email = email;
			FirstName = firstName;
			LastName = lastName;
			AccountState = AccountState.Pending;
		}

		public string UserName { get; set; }

		public string Passowrd { get; set; }

		public string Email { get; set; }

		public string FirstName { get; set; }

		public AccountState AccountState { get; set; }

		public string LastName { get; set; }
	}
}
