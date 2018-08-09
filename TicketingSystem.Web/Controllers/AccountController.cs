using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TicketingSystem.Services;
using TicketingSystem.Web.Models.Account;

namespace TicketingSystem.Web.Controllers
{
	public class AccountController : Controller
	{
		private readonly IUserService _userService;

		public AccountController(IUserService userService)
		{
			_userService = userService ?? throw new ArgumentNullException(nameof(userService));
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			try
			{
				model.ErrorMessage = "";

				LoginResult loginResult = _userService.Login(model.Username, model.Password);

				string role;
				if (loginResult.IsAdministrator)
				{
					role = "Administrator";
				}
				else if (loginResult.IsSupport)
				{
					role = "Support";
				}
				else
				{
					role = "Client";
				}

				var claims = new List<Claim>
				{
				new Claim(ClaimTypes.Name, loginResult.Username),
				new Claim(ClaimTypes.Role, role),
				new Claim("UserId", loginResult.UserId.ToString()),
				};

				var claimsIdentity = new ClaimsIdentity(
					claims, CookieAuthenticationDefaults.AuthenticationScheme);
				var authProperties = new AuthenticationProperties
				{
					//AllowRefresh = <bool>,
					// Refreshing the authentication session should be allowed.

					//ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
					// The time at which the authentication ticket expires. A 
					// value set here overrides the ExpireTimeSpan option of 
					// CookieAuthenticationOptions set with AddCookie.

					//IsPersistent = true,
					// Whether the authentication session is persisted across 
					// multiple requests. Required when setting the 
					// ExpireTimeSpan option of CookieAuthenticationOptions 
					// set with AddCookie. Also required when setting 
					// ExpiresUtc.

					//IssuedUtc = <DateTimeOffset>,
					// The time at which the authentication ticket was issued.

					//RedirectUri = <string>
					// The full path or absolute URI to be used as an http 
					// redirect response value.
				};

				await HttpContext.SignInAsync(
					CookieAuthenticationDefaults.AuthenticationScheme,
					new ClaimsPrincipal(claimsIdentity),
					authProperties);

				return RedirectToAction(nameof(HomeController.Index), "Home");
			}
			catch (Exception e)
			{
				model.ErrorMessage = e.Message;
				return View(model);
			}
		}


		[HttpGet]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			return RedirectToAction(nameof(HomeController.Index), "Home");
		}

		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Register(RegisterViewModel viewModel)
		{
			var model = new CreateUserModel(viewModel.Username, viewModel.Password, viewModel.Email, viewModel.FirstName, viewModel.LastName);

			try
			{
				_userService.Create(model);
			}
			catch (Exception e)
			{
				viewModel.Error = e.Message;
				return View(viewModel);
			}


			return View("FinishRegister", viewModel);
		}

		[HttpGet]
		[Authorize]
		public IActionResult UsersToProcess()
		{
			if (User.IsInRole("Administrator"))
			{
				var usersToApprove = new List<ApprovingUsersViewModel>();

				CreateUnApprovedUserList(usersToApprove, _userService.GetUnApprovedUsers());

				return View(usersToApprove);
			}

			return RedirectToAction(nameof(HomeController.Index), "Home");
		}

		[HttpPost]
		[Authorize]
		public IActionResult Approve(int id, ApprovingUsersViewModel viewModel)
		{
			_userService.Approve(id);

			return View(viewModel);
		}

		[HttpPost]
		[Authorize]
		public IActionResult Deny(int id)
		{
			_userService.Deny(id);

			return RedirectToAction(nameof(UsersToProcess));
		}

		private void CreateUnApprovedUserList(List<ApprovingUsersViewModel> usersToApprove, IEnumerable<User> users)
		{
			foreach (var user in users)
			{

				var viewModel = new ApprovingUsersViewModel
				{
					Id = user.Id,
					Username = user.Username,
					FirstName = user.FirstName,
					LastName = user.LastName,
					Email = user.Email,
				};

				usersToApprove.Add(viewModel);
			}
		}
	}
}
