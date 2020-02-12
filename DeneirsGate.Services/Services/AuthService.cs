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
                        var userRole = db.UserRoles.FirstOrDefault(y => y.UserFK == _user.Id);
                        if (userRole == null)
                        {
                            userRole = new UserRole
                            {
                                RoleFK = db.Roles.FirstOrDefault(x => x.Name == "Player").Id,
                                UserFK = _user.Id
                            };
                            db.UserRoles.Add(userRole);
                        }

                        var role = db.Roles.FirstOrDefault(x => x.Id == userRole.RoleFK);
                        user = new UserDataModel
                        {
                            Username = _user.Username,
                            DisplayName = _user.DisplayName,
                            Priviledge = role.Priviledge,
                            Role = role.Id,
                            UserId = _user.Id
                        };
                    }
                    else { }
                }
            }
            catch (Exception ex)
            {

            }

            return user;
        }

        //public UserDataModel GetUser(Guid id)
        //{
        //    var user = new UserDataModel();
        //    using (var db = new DataEntities())
        //    {
        //        user = db.Users.Where(x => x.Id == id).Select(x => new UserDataModel
        //        {
        //            Priviledge = x.P
        //        })
        //    }
        //}

        public List<RoleDataModel> GetRoles()
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
    }
}
