using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using TicketingSystem.Services;
using TicketingSystem.Services.Impl;
using TicketingSystem.Web.Models.Message;
using TicketingSystem.Web.Models.Ticket;

namespace TicketingSystem.Web.Controllers
{
	public class MessageController : Controller
	{
		public readonly ITicketService _ticketService = new TicketService();

		public readonly IUserService _userService = new UserService();

		public readonly IMessageService _messageService = new MessageService();

		public MessageController(ITicketService ticketService, IProjectService projectService,
			IUserService userService, IMessageService messageService)
		{
			_ticketService = ticketService;
			_userService = userService;
			_messageService = messageService;
		}

		[HttpPost]
		[Authorize]
		public IActionResult Create(int id, string state, ViewTicketViewModel viewModel)
		{
			Ticket ticket = _ticketService.GetByTicketId(id);

			int currnetUserId = _userService.GetByUsername(User.Identity.Name).Id;

			if (User.IsInRole("Client") && currnetUserId != ticket.SubmitterId)
			{
				return NotFound();
			}

			var model = new CreateMessageModel
			{
				TicketId = id,
				UserId = currnetUserId,
				PublishingDate = DateTime.UtcNow,
				Content = viewModel.MessageContent,
				State = state
			};

			_messageService.Create(model);

			return RedirectToAction($"{nameof(TicketController.View)}", "Ticket", new { id });
		}

		[HttpGet]
		[Authorize]
		public IActionResult Edit(int id)
		{
			Message message = _messageService.GetById(id);
			var model = new EditMessageViewModel
			{
				Id = id,
				MessageContent = message.Content
			};

			return View(model);
		}

		[HttpPost]
		[Authorize]
		public IActionResult Edit(EditMessageViewModel viewModel)
		{
			var model = new EditMessageModel
			{
				Id = viewModel.Id,
				MessageContent = viewModel.MessageContent
			};

			int id = _messageService.GetById(viewModel.Id).TicketId;

			_messageService.Edit(model);

			return RedirectToAction($"{nameof(TicketController.View)}", "Ticket", new { id });
		}

		[HttpPost]
		[Authorize]
		public IActionResult Delete(int Id)
		{
			int id = _messageService.GetById(Id).TicketId;

			_messageService.Delete(Id);

			return RedirectToAction($"{nameof(TicketController.View)}", "Ticket", new { id });
		}

		[HttpPost]
		[Authorize]
		public IActionResult ChangeState(int Id)
		{
			int id = _messageService.GetById(Id).TicketId;

			_messageService.ChangeStateToPost(Id);

			return RedirectToAction($"{nameof(TicketController.View)}", "Ticket", new { id });
		}
	}
}
