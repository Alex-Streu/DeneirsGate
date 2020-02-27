using DeneirsGate.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DeneirsGate.Services
{
    public class AuthService : DeneirsService
    {
        public UserDataModel Authenticate(string username, string password)
        {
            UserDataModel user = null;
            try
            {
                using (DBReset())
                {
                    var _user = DB.Users.FirstOrDefault(x => x.Username == username);
                    if (_user != null && Regex.Unescape(_user.Password) == password)
                    {
                        _user.LastLogin = DateTime.Now;
                        DB.SaveChanges();

                        user = GetUserData(_user.Id);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return user;
        }

        public List<RoleDataModel> GetDataRoles()
        {
            var roles = new List<RoleDataModel>();

            using (DBReset())
            {
                roles = DB.Roles.Select(x => new RoleDataModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Priviledge = x.Priviledge
                }).ToList();
            }

            return roles;
        }

        public List<RoleViewModel> GetRoles()
        {
            var roles = new List<RoleViewModel>();
            using (DBReset())
            {
                roles = DB.Roles.Where(x => x.Name != "Admin").Select(x => new RoleViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Priviledge = x.Priviledge
                }).OrderBy(x => x.Priviledge).ToList();
            }

            return roles;
        }

        public void AddUser(RegisterViewModel model)
        {
            using (DBReset())
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    DisplayName = model.Username,
                    Email = model.Email,
                    Password = model.Password,
                    Username = model.Username.ToLower(),
                    Picture = model.Picture,
                    LastLogin = DateTime.Now
                };

                DB.Users.Add(user);
                DB.UserRoles.Add(new UserRole
                {
                    RoleFK = model.Role,
                    UserFK = user.Id
                });
                DB.SaveChanges();
            }
        }

        public UserDataModel GetUserData(string id)
        {
            var _id = Guid.Parse(id);
            return GetUserData(_id);
        }

        public UserDataModel GetUserData(Guid id)
        {
            var user = new UserDataModel();
            using (DBReset())
            {
                user = DB.Users.Where(x => x.Id == id).Select(x => new UserDataModel
                {
                    DisplayName = x.DisplayName,
                    UserId = id,
                    Username = x.Username,
                    Picture = x.Picture,
                    Email = x.Email
                }).FirstOrDefault();

                var userRole = DB.UserRoles.FirstOrDefault(y => y.UserFK == id);
                if (userRole == null)
                {
                    userRole = new UserRole
                    {
                        RoleFK = DB.Roles.FirstOrDefault(x => x.Name == "Player").Id,
                        UserFK = id
                    };
                    DB.UserRoles.Add(userRole);
                    DB.SaveChangesAsync();
                }

                var role = DB.Roles.FirstOrDefault(x => x.Id == userRole.RoleFK);
                user.Priviledge = role.Priviledge;
                user.Role = role.Id;
            }

            return user;
        }

        public PlayerRegistryViewModel GetPlayerRegistry(string code)
        {
            var registry = new PlayerRegistryViewModel();
            var remaining = code.Length % 4;
            for (var i = 0; i < remaining; i++)
            {
                code += "=";
            }

            var characterKey = new Guid(Convert.FromBase64String(code));

            using (DBReset())
            {
                registry = DB.Characters.Where(x => x.CharacterKey == characterKey).Select(x => new PlayerRegistryViewModel
                {
                    CharacterKey = characterKey,
                    Name = x.FirstName + " " + x.LastName
                }).FirstOrDefault();

                registry.CampaignKey = DB.CampaignCharacterLinkers.Where(x => x.CharacterKey == characterKey).Select(x => x.CampaignKey).FirstOrDefault();
            }

            return registry;
        }

        public void UpdatePlayerRegistry(Guid userId, PlayerRegistryPostModel model)
        {
            using (DBReset())
            {
                var registry = DB.CampaignCharacterLinkers.FirstOrDefault(x => x.CharacterKey == model.CharacterKey);
                registry.IsRegistered = true;
                registry.UserKey = userId;

                DB.SaveChanges();
            }
        }
    }
}
