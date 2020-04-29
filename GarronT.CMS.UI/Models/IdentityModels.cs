using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;

namespace GarronT.CMS.UI.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    //public class ApplicationUser : IdentityUser
    //{
    //    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
    //    {
    //        // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
    //        var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
    //        // Add custom user claims here
    //        return userIdentity;
    //    }
    //}
    ////public class ApplicationUser : IdentityUser
    ////{
    ////    public int IsEmailConfirmed { get; set; }
    ////    public String Token { get; set; }
    ////}
    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    //{
    //    public ApplicationDbContext()
    //        : base("DefaultConnection", throwIfV1Schema: false)
    //    {
    //    }

    //    public static ApplicationDbContext Create()
    //    {
    //        return new ApplicationDbContext();
    //    }


    //    public class UserManager : UserManager<ApplicationUser>
    //    {
    //        public UserManager()
    //            : base(new UserStore<ApplicationUser>(new ApplicationDbContext()))
    //        {
    //        }

    //        public IQueryable<ApplicationUser> GetUsersInRole(ApplicationDbContext context, string roleName)
    //        {
    //            if (context != null && roleName != null)
    //            {
    //                var roles = context.Roles.Where(r => r.Name == roleName);
    //                if (roles.Any())
    //                {
    //                    var roleId = roles.First().Id;
    //                    return from user in context.Users
    //                           where user.Roles.Any(r => r.RoleId == roleId)
    //                           select user;
    //                }
    //            }

    //            return null;
    //        }
    //    }


    //}

    
}