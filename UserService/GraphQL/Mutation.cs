using HotChocolate.AspNetCore.Authorization;
using Library.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UserService.GraphQL
{
    public class Mutation
    {
        public async Task<UserData> RegisterUserAsBuyerAsync(
           RegisterUser input,
           [Service] IndividuProjContext context)
        {
            var user = context.Users.Where(o => o.Username == input.UserName).FirstOrDefault();
            if (user != null)
            {
                return await Task.FromResult(new UserData());
            }
            var newUser = new User
            {
                FullName = input.FullName,
                Email = input.Email,
                Username = input.UserName,
                Password = BCrypt.Net.BCrypt.HashPassword(input.Password) // encrypt password
            };

            var memberRole = context.Roles.Where(m => m.Name == "BUYER").FirstOrDefault();
            if (memberRole == null)
                throw new Exception("Invalid Role");
            var userRole = new UserRole
            {
                RoleId = memberRole.Id,
                UserId = newUser.Id
            };
            newUser.UserRoles.Add(userRole);

            var profile = new Profile
            {
                UserId = newUser.Id,
                Name = newUser.FullName,
                Address = "",
                City = "",
                Phone = ""
            };
            newUser.Profiles.Add(profile);
            // EF
            var ret = context.Users.Add(newUser);
            await context.SaveChangesAsync();

            return await Task.FromResult(new UserData
            {
                Id = newUser.Id,
                Username = newUser.Username,
                Email = newUser.Email,
                FullName = newUser.FullName
            });
        }

        public async Task<UserData> RegisterUserAsAdminAsync(
           RegisterUser input,
           [Service] IndividuProjContext context)
        {
            var user = context.Users.Where(o => o.Username == input.UserName).FirstOrDefault();
            if (user != null)
            {
                return await Task.FromResult(new UserData());
            }
            var newUser = new User
            {
                FullName = input.FullName,
                Email = input.Email,
                Username = input.UserName,
                Password = BCrypt.Net.BCrypt.HashPassword(input.Password) // encrypt password
            };

            var memberRole = context.Roles.Where(m => m.Name == "ADMIN").FirstOrDefault();
            if (memberRole == null)
                throw new Exception("Invalid Role");
            var userRole = new UserRole
            {
                RoleId = memberRole.Id,
                UserId = newUser.Id
            };
            newUser.UserRoles.Add(userRole);
            // EF
            var ret = context.Users.Add(newUser);
            await context.SaveChangesAsync();

            return await Task.FromResult(new UserData
            {
                Id = newUser.Id,
                Username = newUser.Username,
                Email = newUser.Email,
                FullName = newUser.FullName
            });
        }

        public async Task<UserData> RegisterUserAsManagerAsync(
           RegisterUser input,
           [Service] IndividuProjContext context)
        {
            var user = context.Users.Where(o => o.Username == input.UserName).FirstOrDefault();
            if (user != null)
            {
                return await Task.FromResult(new UserData());
            }
            var newUser = new User
            {
                FullName = input.FullName,
                Email = input.Email,
                Username = input.UserName,
                Password = BCrypt.Net.BCrypt.HashPassword(input.Password) // encrypt password
            };

            var memberRole = context.Roles.Where(m => m.Name == "MANAGER").FirstOrDefault();
            if (memberRole == null)
                throw new Exception("Invalid Role");
            var userRole = new UserRole
            {
                RoleId = memberRole.Id,
                UserId = newUser.Id
            };
            newUser.UserRoles.Add(userRole);
            // EF
            var ret = context.Users.Add(newUser);
            await context.SaveChangesAsync();

            return await Task.FromResult(new UserData
            {
                Id = newUser.Id,
                Username = newUser.Username,
                Email = newUser.Email,
                FullName = newUser.FullName
            });
        }

        public async Task<UserData> RegisterUserAsCourierAsync(
           RegisterUser input,
           [Service] IndividuProjContext context)
        {
            var user = context.Users.Where(o => o.Username == input.UserName).FirstOrDefault();
            if (user != null)
            {
                return await Task.FromResult(new UserData());
            }
            var newUser = new User
            {
                FullName = input.FullName,
                Email = input.Email,
                Username = input.UserName,
                Password = BCrypt.Net.BCrypt.HashPassword(input.Password) // encrypt password
            };

            var memberRole = context.Roles.Where(m => m.Name == "COURIER").FirstOrDefault();
            if (memberRole == null)
                throw new Exception("Invalid Role");
            var userRole = new UserRole
            {
                RoleId = memberRole.Id,
                UserId = newUser.Id
            };
            newUser.UserRoles.Add(userRole);

            var courier = new Courier { UserId = newUser.Id };
            newUser.Couriers.Add(courier);
            // EF
            var ret = context.Users.Add(newUser);
            await context.SaveChangesAsync();

            return await Task.FromResult(new UserData
            {
                Id = newUser.Id,
                Username = newUser.Username,
                Email = newUser.Email,
                FullName = newUser.FullName
            });
        }

        public async Task<UserToken> LoginAsync(
            LoginUser input,
            [Service] IOptions<TokenSettings> tokenSettings, // setting token
            [Service] IndividuProjContext context) // EF
        {
            var user = context.Users.Where(o => o.Username == input.Username).FirstOrDefault();
            if (user == null)
            {
                return await Task.FromResult(new UserToken(null, null, "Username or password was invalid"));
            }
            bool valid = BCrypt.Net.BCrypt.Verify(input.Password, user.Password);
            if (valid)
            {
                // generate jwt token
                var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Value.Key));
                var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

                // jwt payload
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, user.Username));

                var userRoles = context.UserRoles.Where(o => o.UserId == user.Id).ToList();
                foreach (var userRole in userRoles)
                {
                    var role = context.Roles.Where(o => o.Id == userRole.RoleId).FirstOrDefault();
                    if (role != null)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.Name));
                    }
                }

                var expired = DateTime.Now.AddHours(3);
                var jwtToken = new JwtSecurityToken(
                    issuer: tokenSettings.Value.Issuer,
                    audience: tokenSettings.Value.Audience,
                    expires: expired,
                    claims: claims, // jwt payload
                    signingCredentials: credentials // signature
                );

                return await Task.FromResult(
                    new UserToken(new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expired.ToString(), null));
                //return new JwtSecurityTokenHandler().WriteToken(jwtToken);
            }

            return await Task.FromResult(new UserToken(null, null, Message: "Username or password was invalid"));
        }

        [Authorize]
        public async Task<UserData> ChangePassAsync(
           ChangePassUser input,
           [Service] IndividuProjContext context)
        {
            var user = context.Users.Where(o => o.Id == input.Id).FirstOrDefault();
            if (user != null)
            {

                user.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);//encrypt password
               
            }

            // EF
            var ret = context.Users.Update(user);
            await context.SaveChangesAsync();

            return await Task.FromResult(new UserData
            {
                Id = user.Id,
                Username = user.Username
                
            });
            
        }

        [Authorize(Roles = new[] { "ADMIN" })]
        public async Task<User> DeleteUserByIdAsync(
         int id, [Service] IndividuProjContext context)
        {
            var user = context.Users.Where(o => o.Id == id).FirstOrDefault();
            if (user != null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
            }
            return await Task.FromResult(user);
        }

        [Authorize]
        public async Task<Profile> UpdateProfileAsync(
            ProfilesInput input,
            ClaimsPrincipal claimsPrincipal,
            [Service] IndividuProjContext context)
        {
            var username = claimsPrincipal.Identity.Name;
            var user = context.Users.Where(x => x.Username == username).FirstOrDefault();
            var profile = context.Profiles.FirstOrDefault(x => x.UserId == user.Id);
            profile.Name = input.Name;
            profile.City = input.City;
            profile.Address = input.Address;
            profile.Phone = input.Phone;

            var ret = context.Profiles.Update(profile);
            await context.SaveChangesAsync();
            return ret.Entity;
        }
    }
}
