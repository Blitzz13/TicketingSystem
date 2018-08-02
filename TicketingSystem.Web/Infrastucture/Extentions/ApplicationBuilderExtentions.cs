using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TicketingSystem.Data;

namespace CameraBazar.Web.Infrastructure.Extentions
{
	public static class ApplicationBuilderExtentions
	{
		public static IApplicationBuilder UseDatabaseMigrations(this IApplicationBuilder app)
		{
			using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				serviceScope.ServiceProvider.GetService<TicketingSystemDbContext>().Database.Migrate();
			}

			return app;
		}
	}
}
