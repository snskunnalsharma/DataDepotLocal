using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.DAL
{
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }



    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }


        public class UserManager : UserManager<ApplicationUser>
        {
            public UserManager()
                : base(new UserStore<ApplicationUser>(new ApplicationDbContext()))
            {
            }

            public IQueryable<ApplicationUser> GetUsersInRole(string roleName)
            {
                ApplicationDbContext context = new ApplicationDbContext();
                if (context != null && roleName != null)
                {
                    var roles = context.Roles.Where(r => r.Name == roleName);
                    if (roles.Any())
                    {
                        var roleId = roles.First().Id;
                        return from user in context.Users
                               where user.Roles.Any(r => r.RoleId == roleId)
                               select user;
                    }
                }

                return null;
            }
        }


    }
}
