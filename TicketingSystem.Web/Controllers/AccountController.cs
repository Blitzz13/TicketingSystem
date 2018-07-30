using Microsoft.AspNetCore.Mvc;
using System;
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
		private static readonly IMessageService _messageService = new MessageService();
		private static LoggedUser _loggedUser;

		public IActionResult Login() => View();

		[HttpPost]
		public IActionResult Login(LoginViewModel model)
		{
			LoginResult loginResult;

			try
			{
				loginResult = _userService.Login(model.Username, model.Password);
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, ex.Message);
				return View(model);
			}
			
		}
	}
}
