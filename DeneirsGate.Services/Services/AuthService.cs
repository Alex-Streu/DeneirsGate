using DeneirsGate.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DeneirsGate.Services
{
    public class AuthService : DeneirsService
    {
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
