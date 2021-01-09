using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TicketingSystem.Services;
using TicketingSystem.Web.Models.Account;

namespace TicketingSystem.Web.Controllers
{
	public class AccountController : Controller
	{
		private readonly IUserService _userService;

		private const int PageSize = 5;
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
			if (viewModel.AccountState == null)
			{
				viewModel.AccountState = "Pending";
			}

			var model = new CreateUserModel(viewModel.Username, viewModel.Password,
				viewModel.Email, viewModel.FirstName, viewModel.LastName, viewModel.AccountState);

			try
			{
				_userService.Create(model);
			}
			catch (Exception e)
			{
				viewModel.ErrorMessage = e.Message;
				return View(viewModel);
			}


			return View("FinishRegister", viewModel);
		}

		public IActionResult Details(int id)
		{
			var account = _userService.GetByUserId(id);

			if (account.Username != User.Identity.Name && !User.IsInRole("Administrator"))
			{
				return NotFound();
			}

			var viewModel = new DetailsViewModel
			{
				Id = account.Id,
				Email = account.Email,
				Username = account.Username,
				FirstName = account.FirstName,
				LastName = account.LastName,
				Role = account.Role,
				State = account.State
			};

			return View(viewModel);
		}

		[HttpPost]
		[Authorize]
		public IActionResult Edit(int id, DetailsViewModel viewModel)
		{
			if (!User.IsInRole("Administrator"))
			{
				return NotFound();
			}

			var updateModel = new UpdateUserModel
			{
				Username = viewModel.Username,
				Email = viewModel.Email,
				FirstName = viewModel.FirstName,
				LastName = viewModel.LastName,
				Role = viewModel.Role,
				AccountState = viewModel.State
			};

			_userService.Update(id, updateModel);
			return RedirectToAction(nameof(Details), new { id });
		}

		[HttpGet]
		[Authorize]
		public IActionResult Delete(int id)
		{
			User user = _userService.GetByUserId(id);

			int currnetUserId = _userService.GetByUsername(User.Identity.Name).Id;

			if (User.IsInRole("Client"))
			{
				return NotFound();
			}

			if (user != null)
			{
				var model = new DeleteViewModel
				{
					UserId = id,
					Username = user.Username
				};

				return View(model);
			}

			return NotFound();
		}

		[HttpPost]
		[Authorize]
		public IActionResult Delete(int id, DeleteViewModel deleteView)
		{
			_userService.Delete(id);

			return RedirectToAction(nameof(BrowseUsers));
		}

		[HttpGet]
		[Authorize]
		public IActionResult UsersToProcess(int page = 1)
		{
			if (User.IsInRole("Administrator"))
			{
				var usersToShow = new List<ListingUsersViewModel>();

				int usersCount = _userService.GetAllUnApprovedUsersCount();

				CreateUserList(usersToShow, _userService.GetAllUnApprovedUsers(page, PageSize));

				return View(new UsersToProcessListingModel
				{
					Users = usersToShow,
					CurrentPage = page,
					TotalPages = (int)Math.Ceiling(usersCount / (double)PageSize)
				});
			}

			return RedirectToAction(nameof(HomeController.Index), "Home");
		}

		[HttpGet]
		[Authorize]
		public IActionResult BrowseUsers(int page = 1)
		{
			if (User.IsInRole("Administrator"))
			{
				var usersToShow = new List<ListingUsersViewModel>();

				int usersCount = _userService.GetAllProcessedUsersCount();

				CreateUserList(usersToShow, _userService.GetAllProcessedUsers(page, PageSize));

				return View(new UsersToProcessListingModel
				{
					Users = usersToShow,
					CurrentPage = page,
					TotalPages = (int)Math.Ceiling(usersCount / (double)PageSize)
				});
			}

			return NotFound();
		}

		[HttpPost]
		[Authorize]
		public IActionResult Approve(int id)
		{
			_userService.Approve(id);

			return RedirectToAction(nameof(BrowseUsers));
		}

		[HttpPost]
		[Authorize]
		public IActionResult Deny(int id)
		{
			_userService.Deny(id);

			return RedirectToAction(nameof(BrowseUsers));
		}

		private void CreateUserList(List<ListingUsersViewModel> usersToApprove, IEnumerable<User> users)
		{
			foreach (var user in users)
			{

				var viewModel = new ListingUsersViewModel
				{
					Id = user.Id,
					Username = user.Username,
					FirstName = user.FirstName,
					LastName = user.LastName,
					Email = user.Email,
					Role = user.Role,
					State = user.State
				};

				usersToApprove.Add(viewModel);
			}
		}
	}
}
