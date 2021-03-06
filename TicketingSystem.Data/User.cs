﻿using System.Collections.Generic;

namespace TicketingSystem.Data
{
	public class User
	{
		public int Id { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }

		public AccountRole Role { get; set; }

		public string Email { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public AccountState AccountState { get; set; }

		public ICollection<Message> Messages { get; set; } = new List<Message>();

		public ICollection<Project> Projects { get; set; } = new List<Project>();

		public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
	}
}
