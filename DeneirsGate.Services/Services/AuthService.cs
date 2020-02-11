using DeneirsGate.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeneirsGate.Services
{
    public class AuthService
    {
        public bool Authenticate(string username, string password)
        {
            var isAuthed = false;
            using (var db = new DataEntities())
            {
                var user = db.Users.FirstOrDefault(x => x.Username == username);
                user = db.Users.FirstOrDefault();
                if (db.Users.FirstOrDefault(x => x.Username == username && x.Password == password) != null)
                {
                    isAuthed = true;
                }
            }

            return isAuthed;
        }
    }
}
