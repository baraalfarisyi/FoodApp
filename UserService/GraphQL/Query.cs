using HotChocolate.AspNetCore.Authorization;
using Library.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace UserService.GraphQL
{
    public class Query
    {
        [Authorize(Roles = new[] { "ADMIN" })]
        public IQueryable<UserData> GetUsers([Service] IndividuProjContext context) =>
            context.Users.Select(p => new UserData()
            {
                Id = p.Id,
                FullName = p.FullName,
                Email = p.Email,
                Username = p.Username
            });

        public IQueryable<Profile> GetSelfProfiles([Service] IndividuProjContext context, ClaimsPrincipal claimsPrincipal)
        {
            var userName = claimsPrincipal.Identity.Name;
            var user = context.Users.Where(p => p.Username == userName).Include(p => p.Profiles).FirstOrDefault();

            if (user != null)
            {
                var profiles = user.Profiles;
                //var profiles = context.Profiles.Where(p => p.UserId == user.Id);
                return profiles.AsQueryable();

            }


            return new List<Profile>().AsQueryable();
        }
    }
}
