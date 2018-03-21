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
				.HasMany(u => u.Messages)
				.WithOne(m => m.User)
				.HasForeignKey(m => m.UserId);

			builder.Entity<Project>()
				.HasMany(p => p.Tickets)
				.WithOne(t => t.Project)
				.HasForeignKey(t => t.ProjectId);

			builder.Entity<Ticket>()
				.HasMany(t => t.Files)
				.WithOne(f => f.Ticket)
				.HasForeignKey(f => f.TicketId);

			builder.Entity<Message>()
				.HasMany(m => m.Files)
				.WithOne(f => f.Message)
				.HasForeignKey(f => f.MessageId);

			builder.Entity<UserProject>()
				.HasKey(up => new {up.ProjectId, up.UserId});

			builder.Entity<UserProject>()
				.HasOne(up => up.User)
				.WithMany(u => u.UserProjects)
				.HasForeignKey(up => up.UserId);

			builder.Entity<UserProject>()
				.HasOne(up => up.Project)
				.WithMany(p => p.UserProjects)
				.HasForeignKey(up => up.ProjectId);

		}
	}
}
