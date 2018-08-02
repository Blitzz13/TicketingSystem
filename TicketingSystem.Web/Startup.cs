using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicketingSystem.Data;
using TicketingSystem.Services;
using TicketingSystem.Services.Impl;

namespace TicketingSystem.Web
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<TicketingSystemDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

			services.AddMvc();

			services.AddScoped<IUserService, UserService>();
			services.AddScoped<ITicketService, TicketService>();
			services.AddScoped<IProjectService, ProjectService>();
			services.AddScoped<IMessageService, MessageService>();

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseAuthentication();

			app.UseStaticFiles();

			app.UseMvcWithDefaultRoute();
		}
	}
}
