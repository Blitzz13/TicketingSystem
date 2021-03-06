﻿using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TicketingSystem.Data
{
	public class TicketingSystemDbContext : DbContext
	{
		public TicketingSystemDbContext() 
		{
		}

		public TicketingSystemDbContext(DbContextOptions options)
			:base(options)
		{
		}

		public DbSet<User> Users { get; set; }

		public DbSet<Project> Projects { get; set; }

		public DbSet<Ticket> Tickets { get; set; }

		public DbSet<File> Files { get; set; }

		public DbSet<Message> Messages { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder builder)
		{
			base.OnConfiguring(builder);

			if (!builder.IsConfigured)
			{
				builder.UseSqlServer(@"Server=(LocalDb)\MSSQLLocalDB;Database=TicketingSystem;Integrated Security=True");
			}

		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<User>()
				.HasKey(u => u.Id);

			builder.Entity<User>()
				.Property(u => u.Username)
				.IsRequired();

			builder.Entity<User>()
				.HasAlternateKey(u => u.Username)
				.HasName("AlternateKey_Username");

			builder.Entity<User>()
				.HasMany(u => u.Messages)
				.WithOne(m => m.User)
				.HasForeignKey(m => m.UserId);

			builder.Entity<User>()
				.HasMany(u => u.Tickets)
				.WithOne(m => m.Submitter)
				.HasForeignKey(m => m.SubmitterId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Project>()
				.Property(u => u.Name)
				.IsRequired();

			builder.Entity<Project>()
				.HasAlternateKey(u => u.Name)
				.HasName("AlternateKey_ProjectName");

			builder.Entity<Project>()
				.HasOne(p => p.User)
				.WithMany(u => u.Projects)
				.HasForeignKey(p => p.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Ticket>()
				.HasMany(t => t.Files)
				.WithOne(f => f.Ticket)
				.HasForeignKey(f => f.TicketId);

			builder.Entity<Message>()
				.HasMany(m => m.Files)
				.WithOne(f => f.Message)
				.HasForeignKey(f => f.MessageId);
		}
	}
}