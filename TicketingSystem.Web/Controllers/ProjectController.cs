using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using TicketingSystem.Services;
using TicketingSystem.Web.Models.Project;

namespace TicketingSystem.Web.Controllers
{
	public class ProjectController : Controller
	{
		private readonly IProjectService _projectService;

		public ProjectController(IProjectService projectService)
		{
			_projectService = projectService;
		}

		[HttpGet]
		[Authorize]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[Authorize]
		public IActionResult Create(CreateProjectViewModel viewModel)
		{
			HttpContextAccessor accessor = new HttpContextAccessor();

			var claims = accessor.HttpContext.User.Claims;
			int userId = -1;

			foreach (var claim in claims)
			{
				string claimValue = claim.Value;
				int.TryParse(claimValue, out userId);
			}

			var model = new CreateProjectModel(viewModel.Name, viewModel.Description, userId);

			try
			{
				_projectService.Create(model);
			}
			catch (Exception e)
			{
				viewModel.ErrorMessage = e.Message;
				return View(nameof(Create), viewModel);
			}

			return RedirectToAction(nameof(HomeController.Index), "Home");
		}
	}
}
