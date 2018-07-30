using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Services;
using TicketingSystem.Services.Impl;
using TicketingSystem.Web.Models.Ticket;

namespace TicketingSystem.Web.Controllers
{
	public class TicketController : Controller
	{
		public IActionResult Create() => View();

		private static readonly ITicketService _ticketService = new TicketService();

		[HttpPost]
		public IActionResult Create(CreateTicketViewModel viewModel)
		{
			if (!ModelState.IsValid)
			{
				return View(viewModel);
			}

			var model = new CreateTicketModel
			{

			};

			_ticketService.Create(model);

			return RedirectToAction(nameof(HomeController.Index),"Index");
		}
	}
}
