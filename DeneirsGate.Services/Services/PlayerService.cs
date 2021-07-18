using CSharpVitamins;
using DeneirsGate.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeneirsGate.Services
{
    public enum PlayerInviteTo
    {
        Player, Owner
    }

    public class PlayerService : DeneirsService
    {
        public PlayerService(DataEntities _db)
        {
            db = _db;
        }

        public List<Guid> GetCharacters(Guid userId)
        {
            return db.CampaignCharacterLinkers.Where(x => x.UserKey == userId).Select(x => x.CharacterKey).ToList();
        }

        public Guid SendInvite(Guid userId, PlayerInvitePostModel model)
        {
            var requestKey = Guid.NewGuid();

            db.UserCharacterRequests.Add(new UserCharacterRequest
            {
                CharacterShortKey = model.CharacterShortKey,
                OwnerUserKey = model.SendTo == PlayerInviteTo.Player ? userId : model.UserKey,
                PlayerUserKey = model.SendTo == PlayerInviteTo.Owner ? userId : model.UserKey,
                RequestKey = requestKey,
                SentTo = (int)model.SendTo
            });

            db.SaveChanges();

            return requestKey;
        }

        public Guid RespondToInvite(PlayerInviteResponseModel model)
        {
            var request = db.UserCharacterRequests.FirstOrDefault(x => x.RequestKey == model.RequestKey);
            if (request == null) { throw new Exception("Player Request not found!"); }

            if (model.IsAccepted)
            {
                var characterKey = (ShortGuid)request.CharacterShortKey;
                var character = db.CampaignCharacterLinkers.FirstOrDefault(x => x.CharacterKey == characterKey.Guid);
                character.IsRegistered = true;
                character.UserKey = request.PlayerUserKey;
            }

            var respondToUserKey = request.SentTo == (int)PlayerInviteTo.Owner ? request.PlayerUserKey : request.OwnerUserKey;
            db.UserCharacterRequests.Remove(request);

            db.SaveChanges();

            return respondToUserKey;
        }
    }
}
