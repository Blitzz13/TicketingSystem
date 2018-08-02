using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using TicketingSystem.Services;
using TicketingSystem.Services.Impl;
using TicketingSystem.Web.Models.Ticket;

namespace TicketingSystem.Web.Controllers
{
	public class TicketController : Controller
	{
		private static readonly ITicketService _ticketService = new TicketService();

		private static readonly IProjectService _projectService = new ProjectService();

		[HttpGet]
		[Authorize]
		public IActionResult Create()
		{
			CreateTicketViewModel model = new CreateTicketViewModel();
			List<string> projectNames = _projectService.Get().Select(pr => pr.Name).ToList();

			model.ProjectNames = new List<string>();

			foreach (var name in projectNames)
			{
				model.ProjectNames.Add(name);
			}
			
			return View(model);
		}

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

			return View();
		}
	}
}
