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
                        if (DB.Campaigns.FirstOrDefault(x => x.CampaignKey == contentKey) == null) { hasAccess = true; break; }

                        if (DB.UserCampaigns.FirstOrDefault(x => x.UserKey == userId && x.CampaignKey == contentKey && x.IsOwner) != null)
                        {
                            hasAccess = true;
                        }
                        break;

                    case ContentType.Character:
                        //Check if exists
                        if (DB.Characters.FirstOrDefault(x => x.CharacterKey == contentKey) == null) { hasAccess = true; break; }

                        //Check user of player
                        if (DB.CampaignCharacterLinkers.FirstOrDefault(x => x.IsRegistered && x.UserKey == userId && x.CharacterKey == contentKey) != null)
                        {
                            hasAccess = true;
                            break;
                        }

                        //Check campaign owner
                        var campaignKeys = DB.UserCampaigns.Where(x => x.UserKey == userId && x.IsOwner).Select(x => x.CampaignKey).ToList();
                        if (campaignKeys == null) { break; }
                        if (DB.CampaignCharacterLinkers.FirstOrDefault(x => campaignKeys.Contains(x.CampaignKey) && x.CharacterKey == contentKey) != null)
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
            dashboard.NPCs = GetCharacterShorts(userId, campaignId).OrderByDescending(x => x.LastUpdateDate).Take(8).ToList();

            return dashboard;
        }

        public List<CharacterShortViewModel> GetAllCharacters(Guid userId, Guid campaignId)
        {
            UserHasAccess(userId, campaignId, ContentType.Campaign);

            var characters = new List<CharacterShortViewModel>();
            using (DBReset())
            {
                var _players = DB.CampaignCharacterLinkers.Where(x => x.CampaignKey == campaignId).ToList();
                foreach (var _p in _players)
                {
                    var addPlayer = GetPlayerShort(userId, campaignId, _p.CharacterKey, _p.UserKey);

                    if (addPlayer != null) { characters.Add(addPlayer); }
                }
            }

            return characters.OrderBy(x => x.FirstName).ToList();
        }

        public List<CharacterShortViewModel> GetCharacterShorts(Guid userId, Guid campaignId)
        {
            UserHasAccess(userId, campaignId, ContentType.Campaign);

            var characters = new List<CharacterShortViewModel>();
            using (DBReset())
            {
                var _players = DB.CampaignCharacterLinkers.Where(x => !x.IsPlayer && x.CampaignKey == campaignId).ToList();
                foreach (var _p in _players)
                {
                    var addPlayer = GetPlayerShort(userId, campaignId, _p.CharacterKey, _p.UserKey);

                    if (addPlayer != null) { characters.Add(addPlayer); }
                }
            }

            return characters;
        }

        public List<PlayerViewModel> GetPlayers(Guid userId, Guid campaignId)
        {
            UserHasAccess(userId, campaignId, ContentType.Campaign);

            var players = new List<PlayerViewModel>();
            using (DBReset())
            {
                var _players = DB.CampaignCharacterLinkers.Where(x => x.CampaignKey == campaignId && x.IsPlayer).ToList();
                foreach (var _p in _players)
                {
                    var addPlayer = GetPlayer(userId, campaignId, _p.CharacterKey, _p.UserKey);

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
                var _players = DB.CampaignCharacterLinkers.Where(x => x.IsPlayer && x.CampaignKey == campaignId).ToList();
                foreach (var _p in _players)
                {
                    var addPlayer = GetPlayerShort(userId, campaignId, _p.CharacterKey, _p.UserKey);

                    if (addPlayer != null) { players.Add(addPlayer); }
                }
            }

            return players;
        }

        public PlayerViewModel GetPlayer(Guid userId, Guid campaignId, Guid characterId, Guid? userKey = null)
        {
            var player = CreatePlayer(campaignId, characterId);

            if (player == null)
            {
                UserHasAccess(userId, characterId, ContentType.Character);

                using (DBReset())
                {
                    if (userKey == null)
                    {
                        var linker = DB.CampaignCharacterLinkers.FirstOrDefault(x => x.CampaignKey == campaignId && x.CharacterKey == characterId);
                        if (linker.IsRegistered) { userKey = linker.UserKey; }
                    }

                    player = DB.Characters.Where(x => x.CharacterKey == characterId).Select(x => new PlayerViewModel
                    {
                        Abilities = x.Abilities,
                        Alignment = x.Alignment,
                        BackgroundKey = x.BackgroundKey,
                        Backstory = x.Backstory,
                        CharacterKey = x.CharacterKey,
                        Charisma = x.Charisma,
                        ClassKey = x.ClassKey,
                        Constitution = x.Constitution,
                        Dexterity = x.Dexterity,
                        Fears = x.Fears,
                        FirstName = x.FirstName,
                        Ideals = x.Ideals,
                        Intelligence = x.Intelligence,
                        Languages = x.Languages,
                        LastName = x.LastName,
                        Level = x.Level,
                        MaxHP = x.MaxHP,
                        Portrait = x.Portrait,
                        RaceKey = x.RaceKey,
                        Status = x.Status,
                        Strength = x.Strength,
                        Wisdom = x.Wisdom,
                        CampaignKey = campaignId,
                        Armor = x.Armor,
                        ArmorClass = x.ArmorClass,
                        Proficiency = x.Proficiency,
                        SpellcastingAbility = x.SpellcastingAbility,
                        SpellcastingMod = x.SpellcastingMod,
                        SpellSaveDC = x.SpellSaveDC,
                        Cantrips = x.Cantrips,
                        Level1Spells = x.Level1Spells,
                        Level2Spells = x.Level2Spells,
                        Level3Spells = x.Level3Spells,
                        Level4Spells = x.Level4Spells,
                        Level5Spells = x.Level5Spells,
                        Level6Spells = x.Level6Spells,
                        Level7Spells = x.Level7Spells,
                        Level8Spells = x.Level8Spells,
                        Level9Spells = x.Level9Spells,
                        Copper = x.Copper,
                        Silver = x.Silver,
                        Electrum = x.Electrum,
                        Gold = x.Gold,
                        Platinum = x.Platinum,
                        Inventory = x.Inventory,
                        Weapons = DB.CharacterWeapons.Where(y => y.CharacterKey == characterId).Select(y => new CharacterWeaponViewModel
                        {
                            AttackMod = y.AttackMod,
                            DamageDice = y.DamageDice,
                            DamageMod = y.DamageMod,
                            DamageType = y.DamageType,
                            Name = y.Name,
                            WeaponKey = y.WeaponKey
                        }).ToList(),
                        Spells = DB.CharacterSpells.Where(y => y.CharacterKey == characterId).Select(y => new CharacterSpellViewModel
                        {
                            Level = y.Level,
                            Name = y.Name,
                            SpellKey = y.SpellKey
                        }).ToList(),
                        LastUpdateDate = x.LastUpdateDate
                    }).FirstOrDefault();

                    player.UserKey = userKey ?? Guid.Empty;
                    player.UserCode = Convert.ToBase64String(characterId.ToByteArray()).Replace("=", "");

                    if (player.UserKey != Guid.Empty)
                    {
                        player.UserName = DB.AspNetUsers.Where(x => x.UserId == player.UserKey).Select(x => x.UserName).FirstOrDefault();
                    }
                }
            }

            return player;
        }

        public PlayerShortViewModel GetPlayerShort(Guid userId, Guid campaignId, Guid characterId, Guid? userKey = null)
        {
            UserHasAccess(userId, characterId, ContentType.Character);

            var player = new PlayerShortViewModel();

            using (DBReset())
            {
                if (userKey == null) { userKey = DB.CampaignCharacterLinkers.FirstOrDefault(x => x.CampaignKey == campaignId && x.CharacterKey == characterId).UserKey; }
                player = DB.Characters.Where(x => x.CharacterKey == characterId).Select(x => new PlayerShortViewModel
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Level = x.Level,
                    Portrait = x.Portrait,
                    CampaignKey = campaignId,
                    CharacterKey = characterId,
                    Race = DB.Races.FirstOrDefault(y => y.RaceKey == x.RaceKey).Name,
                    Class = DB.Classes.FirstOrDefault(y => y.ClassKey == x.ClassKey).Name,
                    UserKey = userKey.Value,
                    LastUpdateDate = x.LastUpdateDate
                }).FirstOrDefault();
            }

            return player;
        }

        public CharacterViewModel CreateCharacter(Guid campaignId, Guid characterId)
        {
            var exists = false;
            using (DBReset())
            {
                if (DB.CampaignCharacterLinkers.FirstOrDefault(x => x.CampaignKey == campaignId && x.CharacterKey == characterId) != null)
                {
                    exists = true;
                }
            }
            
            if (exists) { return null; }

            return new CharacterViewModel
            {
                CampaignKey = campaignId,
                CharacterKey = characterId
            };
        }

        public PlayerViewModel CreatePlayer(Guid campaignId, Guid characterId)
        {
            var exists = false;
            using (DBReset())
            {
                if (DB.CampaignCharacterLinkers.FirstOrDefault(x => x.CampaignKey == campaignId && x.CharacterKey == characterId) != null)
                {
                    exists = true;
                }
            }

            if (exists) { return null; }

            return new PlayerViewModel
            {
                CampaignKey = campaignId,
                CharacterKey = characterId
            };
        }

        public void UpdateCharacter(Guid userId, CharacterPostModel model, bool isPlayer = false, Guid? userKey = null)
        {
            UserHasAccess(userId, model.CharacterKey, ContentType.Character);

            using (DBReset())
            {
                var add = false;
                var character = DB.Characters.FirstOrDefault(x => x.CharacterKey == model.CharacterKey);

                if (character == null)
                {
                    character = new Character();
                    add = true;
                }

                character.Abilities = model.Abilities;
                character.Charisma = model.Charisma;
                character.Constitution = model.Constitution;
                character.Dexterity = model.Dexterity;
                character.Intelligence = model.Intelligence;
                character.Level = model.Level;
                character.MaxHP = model.MaxHP;
                character.Status = model.Status;
                character.Strength = model.Strength;
                character.Wisdom = model.Wisdom;
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
                character.ArmorClass = model.ArmorClass;
                character.Armor = model.Armor;
                character.Proficiency = model.Proficiency;
                character.SpellcastingAbility = model.SpellcastingAbility;
                character.SpellcastingMod = model.SpellcastingMod;
                character.SpellSaveDC = model.SpellSaveDC;
                character.Cantrips = model.Cantrips;
                character.Level1Spells = model.Level1Spells;
                character.Level2Spells = model.Level2Spells;
                character.Level3Spells = model.Level3Spells;
                character.Level4Spells = model.Level4Spells;
                character.Level5Spells = model.Level5Spells;
                character.Level6Spells = model.Level6Spells;
                character.Level7Spells = model.Level7Spells;
                character.Level8Spells = model.Level8Spells;
                character.Level9Spells = model.Level9Spells;
                character.Copper = model.Copper;
                character.Silver = model.Silver;
                character.Electrum = model.Electrum;
                character.Gold = model.Gold;
                character.Platinum = model.Platinum;
                character.Inventory = model.Inventory;
                character.LastUpdateDate = DateTime.Today;

                //Weapons
                DB.CharacterWeapons.RemoveRange(DB.CharacterWeapons.Where(x => x.CharacterKey == model.CharacterKey).ToList());
                foreach (var item in model.Weapons)
                {
                    DB.CharacterWeapons.Add(new CharacterWeapon
                    {
                        AttackMod = item.AttackMod,
                        CharacterKey = model.CharacterKey,
                        DamageDice = item.DamageDice,
                        DamageMod = item.DamageMod,
                        DamageType = item.DamageType,
                        Name = item.Name,
                        WeaponKey = item.WeaponKey
                    });
                }

                //Spells
                DB.CharacterSpells.RemoveRange(DB.CharacterSpells.Where(x => x.CharacterKey == model.CharacterKey).ToList());
                foreach (var item in model.Spells)
                {
                    DB.CharacterSpells.Add(new CharacterSpell
                    {
                        CharacterKey = model.CharacterKey,
                        Level = item.Level,
                        Name = item.Name,
                        SpellKey = item.SpellKey
                    });
                }

                if (add)
                {
                    DB.Characters.Add(character);

                    //Add linkers
                    DB.CampaignCharacterLinkers.Add(new CampaignCharacterLinker
                    {
                        CampaignKey = model.CampaignKey,
                        CharacterKey = model.CharacterKey,
                        IsPlayer = isPlayer,
                        UserKey = userKey ?? Guid.Empty
                    });
                }

                DB.SaveChanges();
            }
        }
    }
}
