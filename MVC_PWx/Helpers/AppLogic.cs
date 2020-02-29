using DeneirsGate.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public static Dictionary<string, int> Priviledges
        {
            get
            {
                return new Dictionary<string, int>()
                {
                    { "Player", (int)Priviledge.Player },
                    { "Dungeon Master", (int)Priviledge.DM },
                    { "Admin", (int)Priviledge.Admin }
                };
            }
        }

        static string passphrase = "TrackerOverweening102076";


        public static string EncryptPassword(string password)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            return System.Text.Encoding.ASCII.GetString(data);
        }

        //public static bool HasPriviledge(Priviledge priviledge)
        //{
        //    var dic = new Dictionary<string, Priviledge>()
        //    {
        //        { "Player", Priviledge.Player },
        //        { "Dungeon Master", Priviledge.DM },
        //        { "Admin", Priviledge.Admin }
        //    };
        //    var user = GetUser();

        //    return user.Priviledge >= (int)priviledge;
        //}

        public static void SetUser(UserDataModel user)
        {
            var str = Json.Encode(user);
            HttpContext.Current.Cache["User"] = StringCipher.Encrypt(str, passphrase);
        }

        public static UserDataModel GetUser()
        {
            var cache = HttpContext.Current.Cache["User"];
            if (cache == null) { return null; }

            var str = StringCipher.Decrypt(cache.ToString(), passphrase);
            var user = Json.Decode<UserDataModel>(str);

            return user;
        }

        #region Content Directories

        public static string GetCampaignContentDir(Guid campaignKey)
        {
            return $"~\\Content\\CampaignImages\\campaign-{campaignKey.ToString()}\\";
        }

        public static string GetCharacterContentDir(Guid campaignKey, Guid characterKey)
        {
            return $"{GetCampaignContentDir(campaignKey)}\\Characters\\{characterKey.ToString()}\\";
        }

        public static string GetIconDir()
        {
            return "~\\Content\\img\\icos\\";
        }

        public static string GetDefaultPortrait()
        {
            return "~\\Content\\img\\avatars\\blank-portrait.png";
        }

        public static string GetDefaultCampaignImage()
        {
            return "~\\Content\\img\\campaigns\\campaign-default.png";
        }

        #endregion
    }
}