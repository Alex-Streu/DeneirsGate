using DeneirsGate.Services;
using System.Web;
using System.Web.Helpers;

namespace MVC_PWx.Helpers
{
    public static class AppLogic
    {
        public enum Priviledge
        {
            Player = 1,
            DM,
            Admin
        }

        static string passphrase = "TrackerOverweening102076";

        public static string EncryptPassword(string password)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            return System.Text.Encoding.ASCII.GetString(data);
        }

        public static bool HasPriviledge(Priviledge priviledge)
        {
            var user = GetUser();

            return user.Priviledge >= (int)priviledge;
        }

        public static void SetUser(UserDataModel user)
        {
            var str = Json.Encode(user);
            HttpContext.Current.Cache["User"] = StringCipher.Encrypt(str, passphrase);
        }

        public static UserDataModel GetUser()
        {
            var str = StringCipher.Decrypt(HttpContext.Current.Cache["User"].ToString(), passphrase);
            var user = Json.Decode<UserDataModel>(str);

            return user;
        }
    }
}