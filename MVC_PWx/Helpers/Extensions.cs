

using Microsoft.AspNet.Identity;
using MVC_PWx.Models;
using System.Linq;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public static class ListExtensions
    {
        public static void Move<T>(this List<T> list, T item, int index)
        {
            list.Remove(item);
            list.Insert(index, item);
        }

        public static void MoveToTop<T>(this List<T> list, T item)
        {
            list.Move(item, 0);
        }
    }
}

namespace System
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return String.IsNullOrEmpty(str);
        }
    }
}


namespace MVC_PWx
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