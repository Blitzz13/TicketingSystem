using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketingSystem.Services;
using TicketingSystem.Services.Impl;
using TicketingSystem.Web.Models;
using TicketingSystem.Web.Models.Account;

namespace TicketingSystem.Web.Controllers
{
	public class AccountController : Controller
	{
		private static readonly IUserService _userService = new UserService();
		private static readonly IProjectService _projectService = new ProjectService();
		private static readonly ITicketService _ticketService = new TicketService();
		private static readonly IMessageService _messageService = new MessageService();
		private static LoggedUser _loggedUser;

		public IActionResult Login() => View();

		[HttpPost]
		public IActionResult Login(LoginViewModel model)
		{
			LoginResult loginResult = _userService.Login(model.Username, model.Password);

			if (loginResult != null)
			{
				_loggedUser = new LoggedUser
				{
					UserId = loginResult.UserId,
					Username = loginResult.Username,
					IsAdministrator = loginResult.IsAdministrator,
					IsClient = loginResult.IsClient,
					IsSupport = loginResult.IsSupport
				};
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}
			else
			{
				ModelState.AddModelError(string.Empty, "Invalid login attempt.");
				return View(model);
			}
		}
	}
}
