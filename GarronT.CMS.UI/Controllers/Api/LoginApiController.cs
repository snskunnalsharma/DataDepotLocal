using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using GarronT.CMS.Model;
using GarronT.CMS.UI.Areas.Global.Models;
using System.Web;

namespace GarronT.CMS.UI.Controllers.Api
{
    [AllowAnonymous]
    [RoutePrefix("api")]
    public class LoginApiController : ApiController
    {
        private ApplicationSignInManager SignInManager
            => HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();

        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login(LoginViewModel model)
        {
            if (model == null)
                return Ok(new { success = false, message = "Invalid request" });

            CompassEntities db = new CompassEntities();

            var activeUser = db.tblUsers
                .FirstOrDefault(u => u.UserName == model.UserName && u.Active == 1);

            if (activeUser == null)
                return Ok(new { success = false, message = "User not active or not found" });

            var result = await SignInManager.PasswordSignInAsync(
                model.UserName,
                model.Password,
                model.RememberMe,
                shouldLockout: false
            );

            if (result == SignInStatus.Success)
            {
                return Ok(new
                {
                    success = true,
                    userId = activeUser.UserID,
                    userName = activeUser.UserName
                });
            }

            return Ok(new { success = false, message = "Invalid username or password" });
        }
    }
}
