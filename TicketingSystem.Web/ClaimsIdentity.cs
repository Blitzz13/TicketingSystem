using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using TicketingSystem.Services;

namespace TicketingSystem.Web
{
	public class ClaimsIdentity : IIdentity
	{
		public ClaimsIdentity(IEnumerable<Claim> claims, string scheme)
		{
			claims = new List<Claim>();
		}

		public async void SignInAsync(LoginResult result)
		{
			string role;

			if (result.IsAdministrator)
			{
				role = "Administrator";
			}
			else if (result.IsSupport)
			{
				role = "Support";
			}
			else
			{
				role = "Clinet";
			}
			
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, result.Username),
				new Claim("Id", result.UserId.ToString()),
				new Claim(ClaimTypes.Role, role),
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

			//await HttpContext.SignInAsync(
			//	CookieAuthenticationDefaults.AuthenticationScheme,
			//	new ClaimsPrincipal(claimsIdentity),
			//	authProperties);
		}


		public string AuthenticationType => throw new NotImplementedException();

		public bool IsAuthenticated => throw new NotImplementedException();

		public string Name => throw new NotImplementedException();

	}
}
