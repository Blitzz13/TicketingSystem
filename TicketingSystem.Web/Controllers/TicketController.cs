using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
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

			GetProjectsName(model);

			return View(model);
		}

		[HttpPost]
		[Authorize]
		public IActionResult Create(CreateTicketViewModel viewModel)
		{
			if (!ModelState.IsValid)
			{
				return View(viewModel);
			}

			var model = new CreateTicketModel();

			HttpContextAccessor accessor = new HttpContextAccessor();

			viewModel.ProjectId = _projectService.GetByName(viewModel.ProjectName).Id;
			var claims = accessor.HttpContext.User.Claims;
			int userId = -1;

			foreach (var claim in claims)
			{
				string claimValue = claim.Value;
				int.TryParse(claimValue, out userId);
			}

			string path = viewModel.FilePath;
			if (!string.IsNullOrEmpty(path))
			{
				byte[] file = System.IO.File.ReadAllBytes(path);
				string fileName = Path.GetFileName(path);

				model = new CreateTicketModel
				{
					ProjectId = viewModel.ProjectId,
					TicketTitle = viewModel.TicketTitle,
					TicketDescription = viewModel.Description,
					TicketState = viewModel.TicketState.Replace(" ", ""),
					TicketType = viewModel.TicketType.Replace(" ", ""),
					FileContent = file,
					FileName = fileName,
					SubmitterId = userId
				};
			}

			model = new CreateTicketModel
			{
				ProjectId = viewModel.ProjectId,
				TicketTitle = viewModel.TicketTitle,
				TicketDescription = viewModel.Description,
				TicketState = viewModel.TicketState.Replace(" ",""),
				TicketType = viewModel.TicketType.Replace(" ", ""),
				SubmitterId = userId
			};

			try
			{
				_ticketService.Create(model);
			}
			catch (System.Exception e)
			{
				viewModel.ErrorMessage = e.Message;
				GetProjectsName(viewModel);
				return View(nameof(Create),viewModel);
			}

			return RedirectToAction(nameof(HomeController.Index));
		}

		private static void GetProjectsName(CreateTicketViewModel viewModel)
		{
			List<string> projectNames = _projectService.Get().Select(pr => pr.Name).ToList();

			viewModel.ProjectNames = new List<string>();

			foreach (var name in projectNames)
			{
				viewModel.ProjectNames.Add(name);
			}
		}
	}
}
