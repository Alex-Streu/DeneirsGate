

using Microsoft.AspNet.Identity;
using DeneirsGateSite.Models;
using System.Linq;
using System.Threading.Tasks;


namespace DeneirsGateSite
{
    public static class IdentityExtensions
    {
        public static async Task<ApplicationUser> FindByNameOrEmailAsync(this UserManager<ApplicationUser> userManager, string usernameOrEmail)
        {
            var username = usernameOrEmail;
            if (usernameOrEmail.Contains("@"))
            {
                var userForEmail = await userManager.FindByEmailAsync(usernameOrEmail);
                if (userForEmail != null)
                {
                    username = userForEmail.UserName;
                }
            }
            return await userManager.FindByNameAsync(username);
        }

        public static ApplicationUser FindById(this UserManager<ApplicationUser> userManager, string id)
        {
            var users = userManager.Users.ToList();
            return users.FirstOrDefault(x => x.Id == id);
        }

        public static ApplicationRole FindById(this RoleManager<ApplicationRole> roleManager, string id)
        {
            var roles = roleManager.Roles.ToList();
            return roles.FirstOrDefault(x => x.Id == id);
        }
    }
}