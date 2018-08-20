using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketingSystem.Web.Models.Ticket
{
	public class TicketListingViewModel
	{
		public IEnumerable<OverviewTicketViewModel> Tickets { get; set; }

		public int CurrentPage { get; set; }

		public int TotalPages { get; set; }

		public int PeviousPage => CurrentPage == 1 ? 1 : CurrentPage - 1;

		public int NextPage => CurrentPage == TotalPages ? TotalPages : CurrentPage + 1;
	}
}
