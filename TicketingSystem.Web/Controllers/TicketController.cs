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
		public readonly ITicketService _ticketService = new TicketService();

		public readonly IProjectService _projectService = new ProjectService();

		public readonly IUserService _userService = new UserService();

		public TicketController(ITicketService ticketService, IProjectService projectService, IUserService userService)
		{
			_ticketService = ticketService;
			_projectService = projectService;
			_userService = userService;
		}

		[HttpGet]
		[Authorize]
		public IActionResult Create()
		{
			TicketFormViewModel model = new TicketFormViewModel();

			GetProjectsName(model);

			return View(model);
		}

		[HttpPost]
		[Authorize]
		public IActionResult Create(TicketFormViewModel viewModel)
		{
			var model = new CreateTicketModel();

			viewModel.ProjectId = _projectService.GetByName(viewModel.ProjectName).Id;
			int userId = GetUserId();

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
				TicketState = viewModel.TicketState.Replace(" ", ""),
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
				return View(nameof(Create), viewModel);
			}

			return RedirectToAction(nameof(HomeController.Index), "Home");
		}

		[HttpGet]
		[Authorize]
		public IActionResult ListTickets()
		{
			int userId = GetUserId();
			var overviewTickets = new List<OverviewTicketViewModel>();
			if (User.IsInRole("Client"))
			{
				var tickets = _ticketService.GetAllTicketsForClient(userId);

				CreateOverviewTickets(overviewTickets, tickets);

				return View(overviewTickets);
			}
			else
			{
				var tickets = _ticketService.GetAllTicketsForAdminAndSupport();

				CreateOverviewTickets(overviewTickets, tickets);

				return View(overviewTickets);
			}

		}

		[HttpPost]
		[Authorize]
		public IActionResult DeleteTicket(int id)
		{
			_ticketService.Delete(id);

			return RedirectToAction(nameof(ListTickets));
		}

		[HttpGet]
		[Authorize]
		public IActionResult Edit(int id)
		{
			Ticket ticket = _ticketService.GetByTicketId(id);

			var model = new TicketFormViewModel
			{
				TicketId = id,
				TicketTitle = ticket.Title,
				Description = ticket.Description,
				FileName = ticket.FileName,
				ProjectName = _projectService.GetById(ticket.ProjectId).Name,
				SubmitterId = ticket.SubmitterId,
				TicketState = ticket.State,
				TicketType = ticket.Type,
			};

			return View(model);
		}

		[HttpPost]
		[Authorize]
		public IActionResult Edit(int id, TicketFormViewModel viewModel)
		{
			var model = new UpdateTicketModel
			{
				Id = id,
				ProjectName = viewModel.ProjectName,
				Title = viewModel.TicketTitle,
				Description = viewModel.Description,
				State = viewModel.TicketState.Replace(" ", ""),
				Type = viewModel.TicketType.Replace(" ", ""),
			};

			try
			{
				_ticketService.Edit(model);
			}
			catch (System.Exception e)
			{
				viewModel.ErrorMessage = e.Message;
				viewModel.ProjectName = _projectService.GetById(_ticketService.GetByTicketId(id).ProjectId).Name;
				return View(viewModel);
			}

			return RedirectToAction(nameof(ListTickets));
		}

		private void CreateOverviewTickets(List<OverviewTicketViewModel> overviewTickets, IEnumerable<Ticket> tickets)
		{
			int messagesCount = 0;
			foreach (var ticket in tickets)
			{
				if (ticket.Messages != null)
				{
					messagesCount = ticket.Messages.Count();
				}

				var viewModel = new OverviewTicketViewModel
				{
					Id = ticket.Id,
					Title = ticket.Title,
					ProjectName = _projectService.GetById(ticket.ProjectId).Name,
					SubmissionDate = ticket.SubmissionDate,
					SumitterName = _userService.GetByUserId(ticket.SubmitterId).Username,
					TicketState = ticket.State,
					MessagesCount = messagesCount
				};
				overviewTickets.Add(viewModel);
			}
		}

		private static int GetUserId()
		{
			HttpContextAccessor accessor = new HttpContextAccessor();
			var claims = accessor.HttpContext.User.Claims;
			int userId = -1;

			foreach (var claim in claims)
			{
				string claimValue = claim.Value;
				int.TryParse(claimValue, out userId);
			}

			return userId;
		}

		private void GetProjectsName(TicketFormViewModel viewModel)
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
