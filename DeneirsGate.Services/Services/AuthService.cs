using DeneirsGate.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DeneirsGate.Services
{
    public class AuthService
    {
        public UserDataModel Authenticate(string username, string password)
        {
            UserDataModel user = null;
            try
            {
                using (var db = new DataEntities())
                {
                    var _user = db.Users.FirstOrDefault(x => x.Username == username);
                    if (_user != null && Regex.Unescape(_user.Password) == password)
                    {
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

            using (var db = new DataEntities())
            {
                roles = db.Roles.Select(x => new RoleDataModel
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
            using (var db = new DataEntities())
            {
                roles = db.Roles.Where(x => x.Name != "Admin").Select(x => new RoleViewModel
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
            using (var db = new DataEntities())
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    DisplayName = model.Username,
                    Email = model.Email,
                    Password = model.Password,
                    Username = model.Username.ToLower(),
                    Picture = model.Picture
                };

                db.Users.Add(user);
                db.UserRoles.Add(new UserRole
                {
                    RoleFK = model.Role,
                    UserFK = user.Id
                });
                db.SaveChanges();
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
            using (var db = new DataEntities())
            {
                user = db.Users.Where(x => x.Id == id).Select(x => new UserDataModel
                {
                    DisplayName = x.DisplayName,
                    UserId = id,
                    Username = x.Username,
                    Picture = x.Picture
                }).FirstOrDefault();

                var userRole = db.UserRoles.FirstOrDefault(y => y.UserFK == id);
                if (userRole == null)
                {
                    userRole = new UserRole
                    {
                        RoleFK = db.Roles.FirstOrDefault(x => x.Name == "Player").Id,
                        UserFK = id
                    };
                    db.UserRoles.Add(userRole);
                    db.SaveChangesAsync();
                }

                var role = db.Roles.FirstOrDefault(x => x.Id == userRole.RoleFK);
                user.Priviledge = role.Priviledge;
                user.Role = role.Id;
            }

            return user;
        }
    }
}
