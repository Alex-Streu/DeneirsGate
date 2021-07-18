using DeneirsGate.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DeneirsGate.Services
{
    public class AuthService : DeneirsService
    {
        public AuthService(DataEntities _db)
        {
            db = _db;
        }

        public void SetPasswordResetCode(Guid userId, string code)
        {
            if (userId == Guid.Empty || code.IsNullOrEmpty()) { return; }
            ClearPasswordReset(userId);

            db.UserPasswordResets.Add(new UserPasswordReset
            {
                Code = code,
                UserKey = userId,
                DateCreated = DateTime.Now
            });

            db.SaveChanges();
        }

        public Guid GetPasswordResetUser(string code)
        {
            if (code.IsNullOrEmpty()) { throw new Exception("No reset code provided."); }
            
            var user = db.UserPasswordResets.FirstOrDefault(x => x.Code == code);
            if (user == null)
            {
                throw new Exception("Invalid reset code provided.");
            }

            if (user.DateCreated.AddHours(1) < DateTime.Now)
            {
                throw new Exception("Reset code has expired.");
            }

            return user.UserKey;
        }

        public void ClearPasswordReset(Guid userId)
        {
            db.UserPasswordResets.RemoveRange(x => x.UserKey == userId);
            db.SaveChanges();
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

            registry = db.Characters.Where(x => x.CharacterKey == characterKey).Select(x => new PlayerRegistryViewModel
            {
                CharacterKey = characterKey,
                Name = x.FirstName + " " + x.LastName
            }).FirstOrDefault();

            registry.CampaignKey = db.CampaignCharacterLinkers.Where(x => x.CharacterKey == characterKey).Select(x => x.CampaignKey).FirstOrDefault();

            return registry;
        }

        public void UpdatePlayerRegistry(Guid userId, PlayerRegistryPostModel model)
        {
            var registry = db.CampaignCharacterLinkers.FirstOrDefault(x => x.CharacterKey == model.CharacterKey);
            registry.IsRegistered = true;
            registry.UserKey = userId;

            db.SaveChanges();
        }
    }
}
