using DeneirsGate.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeneirsGate.Services
{
    public class CampaignService : DeneirsService
    {

        private enum ContentType
        {
            Campaign,
            Player,
            Character
        };

        private void UserHasAccess(Guid userId, Guid contentKey, ContentType type)
        {
            var hasAccess = false;
            using (DBReset())
            {
                switch (type)
                {
                    case ContentType.Campaign:
                        //Check if exists
                        if (DB.Campaigns.FirstOrDefault(x => x.CampaignKey == contentKey) == null)
                        {
                            hasAccess = true;
                            break;
                        }

                        if (DB.UserCampaigns.FirstOrDefault(x => x.UserKey == userId && x.CampaignKey == contentKey && x.IsOwner) != null)
                        {
                            hasAccess = true;
                        }
                        break;
                    case ContentType.Player:
                        //Check if exists
                        if (DB.Players.FirstOrDefault(x => x.PlayerKey == contentKey) == null)
                        {
                            hasAccess = true;
                            break;
                        }

                        //Check user of player
                        if (DB.UserPlayerLinkers.FirstOrDefault(x => x.UserKey == userId && x.PlayerKey == contentKey) != null)
                        {
                            hasAccess = true;
                            break;
                        }

                        //Check campaign owner
                        var campaignKeys = DB.UserCampaigns.Where(x => x.UserKey == userId && x.IsOwner).Select(x => x.CampaignKey).ToList();
                        if (campaignKeys == null) { break; }
                        if (DB.CampaignPlayerLinkers.FirstOrDefault(x => campaignKeys.Contains(x.CampaignKey) && x.PlayerKey == contentKey) != null)
                        {
                            hasAccess = true;
                            break;
                        }
                        break;
                    case ContentType.Character:
                        //Check if exists
                        if (DB.Characters.FirstOrDefault(x => x.CharacterKey == contentKey) == null)
                        {
                            hasAccess = true;
                            break;
                        }

                        //Check user of player
                        var players = DB.UserPlayerLinkers.Where(x => x.UserKey == userId).Select(x => x.PlayerKey).ToList();
                        if (DB.CampaignPlayerLinkers.FirstOrDefault(x => players.Contains(x.PlayerKey) && x.CharacterKey == contentKey) != null)
                        {
                            hasAccess = true;
                            break;
                        }

                        //Check campaign owner
                        campaignKeys = db.UserCampaigns.Where(x => x.UserKey == userId && x.IsOwner).Select(x => x.CampaignKey).ToList();
                        if (campaignKeys == null) { break; }
                        if (DB.CampaignPlayerLinkers.FirstOrDefault(x => campaignKeys.Contains(x.CampaignKey) && x.CharacterKey == contentKey) != null)
                        {
                            hasAccess = true;
                            break;
                        }
                        break;
                }
            }

            if (!hasAccess)
            {
                throw new Exception("You do not have access to this content!");
            }
        }

        public List<CampaignViewModel> GetCampaigns(Guid userId, bool isOwner)
        {
            var campaigns = new List<CampaignViewModel>();

            using (DBReset())
            {
                var keys = DB.UserCampaigns.Where(x => x.UserKey == userId && x.IsOwner == isOwner).Select(x => x.CampaignKey).ToList();
                campaigns = DB.Campaigns.Where(x => keys.Contains(x.CampaignKey)).Select(x => new CampaignViewModel
                {
                    CampaignKey = x.CampaignKey,
                    Description = x.Description,
                    Name = x.Name,
                    Portrait = x.Portrait,
                    LastUpdated = x.LastUpdated
                }).OrderByDescending(x => x.LastUpdated).ToList();
            }

            return campaigns;
        }

        public CampaignDashboardViewModel GetCampaignDashboard(Guid userId, Guid campaignId)
        {
            UserHasAccess(userId, campaignId, ContentType.Campaign);

            var dashboard = new CampaignDashboardViewModel();
            dashboard.CampaignKey = campaignId;
            dashboard.Players = GetPlayerShorts(userId, campaignId);

            return dashboard;
        }

        public List<PlayerViewModel> GetPlayers(Guid userId, Guid campaignId)
        {
            UserHasAccess(userId, campaignId, ContentType.Campaign);

            var players = new List<PlayerViewModel>();
            using (DBReset())
            {
                var _players = DB.CampaignPlayerLinkers.Where(x => x.CampaignKey == campaignId).ToList();
                foreach (var _p in _players)
                {
                    var addPlayer = GetPlayer(userId, campaignId, _p.PlayerKey);

                    if (addPlayer != null) { players.Add(addPlayer); }
                }
            }

            return players;
        }

        public List<PlayerShortViewModel> GetPlayerShorts(Guid userId, Guid campaignId)
        {
            UserHasAccess(userId, campaignId, ContentType.Campaign);

            var players = new List<PlayerShortViewModel>();
            using (DBReset())
            {
                var _players = DB.CampaignPlayerLinkers.Where(x => x.CampaignKey == campaignId).ToList();
                foreach (var _p in _players)
                {
                    var addPlayer = GetPlayerShort(userId, campaignId, _p.PlayerKey);

                    if (addPlayer != null) { players.Add(addPlayer); }
                }
            }

            return players;
        }

        public PlayerViewModel GetPlayer(Guid userId, Guid campaignId, Guid playerId)
        {
            var player = CreatePlayer(campaignId, playerId);

            if (player == null)
            {
                UserHasAccess(userId, playerId, ContentType.Player);

                using (DBReset())
                {
                    var p = DB.Players.FirstOrDefault(x => x.PlayerKey == playerId);
                    var cKey = DB.CampaignPlayerLinkers.FirstOrDefault(x => x.CampaignKey == campaignId && x.PlayerKey == playerId)?.CharacterKey;
                    player = DB.Characters.Where(x => x.CharacterKey == cKey).Select(x => new PlayerViewModel
                    {
                        Abilities = p.Abilities,
                        Alignment = x.Alignment,
                        BackgroundKey = x.BackgroundKey,
                        Backstory = x.Backstory,
                        CharacterKey = x.CharacterKey,
                        Charisma = p.Charisma,
                        ClassKey = x.ClassKey,
                        Constitution = p.Constitution,
                        Dexterity = p.Dexterity,
                        Fears = x.Fears,
                        FirstName = x.FirstName,
                        Ideals = x.Ideals,
                        Intelligence = p.Intelligence,
                        Languages = x.Languages,
                        LastName = x.LastName,
                        Level = p.Level,
                        MaxHP = p.MaxHP,
                        PlayerKey = p.PlayerKey,
                        Portrait = x.Portrait,
                        RaceKey = x.RaceKey,
                        Status = p.Status,
                        Strength = p.Strength,
                        Wisdom = p.Wisdom,
                        CampaignKey = campaignId
                    }).FirstOrDefault();
                }
            }

            return player;
        }

        public PlayerShortViewModel GetPlayerShort(Guid userId, Guid campaignId, Guid playerId)
        {
            UserHasAccess(userId, playerId, ContentType.Player);

            var player = new PlayerShortViewModel();

            using (DBReset())
            {
                var p = DB.Players.FirstOrDefault(x => x.PlayerKey == playerId);
                var cKey = DB.CampaignPlayerLinkers.FirstOrDefault(x => x.CampaignKey == campaignId && x.PlayerKey == playerId)?.CharacterKey;
                player = DB.Characters.Where(x => x.CharacterKey == cKey).Select(x => new PlayerShortViewModel
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Level = p.Level,
                    PlayerKey = p.PlayerKey,
                    Portrait = x.Portrait,
                    CampaignKey = campaignId,
                    CharacterKey = cKey.Value,
                    Race = DB.Races.FirstOrDefault(y => y.RaceKey == x.RaceKey).Name,
                    Class = DB.Classes.FirstOrDefault(y => y.ClassKey == x.ClassKey).Name
                }).FirstOrDefault();
            }

            return player;
        }

        public PlayerViewModel CreatePlayer(Guid campaignId, Guid playerId)
        {
            var exists = false;
            using (DBReset())
            {
                if (DB.CampaignPlayerLinkers.FirstOrDefault(x => x.CampaignKey == campaignId && x.PlayerKey == playerId) != null)
                {
                    exists = true;
                }
            }
            
            if (exists) { return null; }

            return new PlayerViewModel
            {
                CampaignKey = campaignId,
                CharacterKey = Guid.NewGuid(),
                PlayerKey = playerId
            };
        }

        public void UpdatePlayer(Guid userId, PlayerPostModel model)
        {
            UserHasAccess(userId, model.PlayerKey, ContentType.Player);

            using (DBReset())
            {
                var add = false;
                var player = db.Players.FirstOrDefault(x => x.PlayerKey == model.PlayerKey);
                var character = db.Characters.FirstOrDefault(x => x.CharacterKey == model.CharacterKey);

                if (player == null)
                {
                    player = new Player();
                    character = new Character();
                    add = true;
                }

                player.Abilities = model.Abilities;
                player.Charisma = model.Charisma;
                player.Constitution = model.Constitution;
                player.Dexterity = model.Dexterity;
                player.Intelligence = model.Intelligence;
                player.Level = model.Level;
                player.MaxHP = model.MaxHP;
                player.PlayerKey = model.PlayerKey;
                player.Status = model.Status;
                player.Strength = model.Strength;
                player.Wisdom = model.Wisdom;

                character.Alignment = model.Alignment;
                character.BackgroundKey = model.BackgroundKey;
                character.Backstory = model.Backstory;
                character.CharacterKey = model.CharacterKey;
                character.ClassKey = model.ClassKey;
                character.Fears = model.Fears;
                character.FirstName = model.FirstName;
                character.Ideals = model.Ideals;
                character.Languages = model.Languages;
                character.LastName = model.LastName;
                character.Portrait = model.Portrait;
                character.RaceKey = model.RaceKey;

                if (add)
                {
                    DB.Players.Add(player);
                    DB.Characters.Add(character);
                    
                    //Add linkers
                    DB.CampaignPlayerLinkers.Add(new CampaignPlayerLinker
                    {
                        CampaignKey = model.CampaignKey,
                        CharacterKey = model.CharacterKey,
                        PlayerKey = player.PlayerKey
                    });
                }

                DB.SaveChanges();
            }
        }
    }
}
