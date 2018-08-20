using System.Collections.Generic;

namespace TicketingSystem.Web.Models.Account
{
	public class UsersToProcessListingModel
	{
		public IEnumerable<ApprovingUsersViewModel> Users { get; set; }

		public int CurrentPage { get; set; }

		public int TotalPages { get; set; }

		public int PeviousPage => CurrentPage == 1 ? 1 : CurrentPage - 1;

		public int NextPage => CurrentPage == TotalPages ? TotalPages : CurrentPage + 1;
	}
}
