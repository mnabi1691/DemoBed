using DemoBed.Base.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DemoBed.Base.Services
{
    public class LoggedInUserService: ILoggedInUserService
    {
        public LoggedInUserService(IHttpContextAccessor context)
        {
            User user = new()
            {
                Id = 0,
                Name = "System",
                Roles = new List<string>()
            };

            this.User = user;

            user.Id = context.HttpContext.User.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => System.Convert.ToInt32(c.Value))
                .Single();

            user.Name = context.HttpContext.User.Claims
                .Where(c => c.Type == ClaimTypes.Name)
                .Select(c => c.Value)
                .Single();

            user.Roles = context.HttpContext.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            if (user.Roles.Contains("Admin"))
            {
                this.IsAdmin = true;
            }
        }

        public User? User { get; }
        public bool? IsAdmin { get; } = false;
    }
}
