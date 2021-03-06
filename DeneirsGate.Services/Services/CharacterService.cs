﻿using DeneirsGate.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using static DeneirsGate.Services.CampaignService;

namespace DeneirsGate.Services
{
    public class CharacterService : DeneirsService
    {
        private void UserHasCharacterAccess(Guid userId, Guid contentKey)
        {
            using (DBReset())
            {
                //Check if exists
                if (DB.Characters.FirstOrDefault(x => x.CharacterKey == contentKey) == null) { return; }

                //Check user of player
                if (DB.CampaignCharacterLinkers.FirstOrDefault(x => x.IsRegistered && x.UserKey == userId && x.CharacterKey == contentKey) != null)
                {
                    return;
                }

                //Check campaign owner
                var campaignKeys = DB.UserCampaigns.Where(x => x.UserKey == userId && x.IsOwner).Select(x => x.CampaignKey).ToList();
                if (DB.CampaignCharacterLinkers.FirstOrDefault(x => campaignKeys.Contains(x.CampaignKey) && x.CharacterKey == contentKey) != null)
                {
                    return;
                }
            }

            throw new Exception("You do not have access to this content!");
        }

        public List<CharacterShortViewModel> GetAllCharacters(Guid userId, Guid campaignId)
        {
            UserHasAccess(userId, campaignId);

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

        public List<ArcCharacterViewModel> GetArcCharacters(Guid userId, Guid campaignId, List<Guid> characterIds)
        {
            UserHasAccess(userId, campaignId);

            var characters = new List<ArcCharacterViewModel>();
            if (characterIds == null) { characterIds = new List<Guid>(); }
            DBReset();

            var _players = DB.CampaignCharacterLinkers.Where(x => !x.IsPlayer && x.CampaignKey == campaignId).ToList();
            foreach (var _p in _players)
            {
                var addPlayer = GetPlayerShort(userId, campaignId, _p.CharacterKey, _p.UserKey);
                var selected = characterIds.Contains(addPlayer.CharacterKey);

                if (addPlayer != null)
                {
                    characters.Add(new ArcCharacterViewModel
                    {
                        CharacterKey = addPlayer.CharacterKey,
                        Class = addPlayer.Class,
                        FirstName = addPlayer.FirstName,
                        LastName = addPlayer.LastName,
                        LastUpdateDate = addPlayer.LastUpdateDate,
                        Level = addPlayer.Level,
                        Portrait = addPlayer.Portrait,
                        Race = addPlayer.Race,
                        IsSelected = selected
                    });
                }
            }

            return characters.OrderBy(x => x.FirstName).ToList();
        }

        public List<PlayerViewModel> GetPlayers(Guid userId, Guid campaignId)
        {
            UserHasAccess(userId, campaignId);

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
            UserHasAccess(userId, campaignId);

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
            UserHasAccess(userId, campaignId);
            UserHasCharacterAccess(userId, characterId);

            var player = CreatePlayer(campaignId, characterId);
            if (player == null)
            {
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
                        Background = DB.Backgrounds.FirstOrDefault(y => y.BackgroundKey == x.BackgroundKey).Name,
                        Backstory = x.Backstory,
                        CharacterKey = x.CharacterKey,
                        Charisma = x.Charisma,
                        ClassKey = x.ClassKey,
                        Class = DB.Classes.FirstOrDefault(y => y.ClassKey == x.ClassKey).Name,
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
                        Race = DB.Races.FirstOrDefault(y => y.RaceKey == x.RaceKey).Name,
                        Status = x.Status,
                        Strength = x.Strength,
                        Wisdom = x.Wisdom,
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
                            DamageDisplay = DB.DamageTypes.FirstOrDefault(z => z.TypeKey == y.DamageType).Name,
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
            UserHasAccess(userId, campaignId);
            UserHasCharacterAccess(userId, characterId);

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
                CharacterKey = characterId
            };
        }

        public void UpdateCharacter(Guid userId, Guid campaignId, CharacterPostModel model, bool isPlayer = false, Guid? userKey = null)
        {
            UserHasAccess(userId, campaignId);
            UserHasCharacterAccess(userId, model.CharacterKey);

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
                        CampaignKey = campaignId,
                        CharacterKey = model.CharacterKey,
                        IsPlayer = isPlayer,
                        UserKey = userKey ?? Guid.Empty
                    });
                }

                DB.SaveChanges();
            }
        }

        public void DeleteCharacter(Guid userId, Guid campaignId, Guid characterId)
        {
            UserHasAccess(userId, campaignId);
            UserHasCharacterAccess(userId, characterId);

            using (DBReset())
            {
                DB.Characters.RemoveRange(x => x.CharacterKey == characterId);
                DB.CampaignCharacterLinkers.RemoveRange(x => x.CharacterKey == characterId);
                DB.CharacterSpells.RemoveRange(x => x.CharacterKey == characterId);
                DB.CharacterWeapons.RemoveRange(x => x.CharacterKey == characterId);
                DB.RelationshipTreeCharacters.RemoveRange(x => x.CharacterKey == characterId);
                DB.ArcCharacterLinkers.RemoveRange(x => x.CharacterKey == characterId);

                DB.SaveChanges();
            }
        }

        public List<ActivityLogViewModel> GetCharacterLogs(Guid userId, Guid campaignId, Guid characterId)
        {
            UserHasAccess(userId, campaignId);
            UserHasCharacterAccess(userId, characterId);

            DBReset();
            var logKeys = DB.CharacterLogs.Where(x => x.CharacterKey == characterId && x.CampaignKey == campaignId).Select(x => x.LogKey).ToList();
            var logs = DB.ActivityLogs.Where(x => logKeys.Contains(x.LogKey)).Select(x => new ActivityLogViewModel
            {
                ArcKey = x.ArcKey,
                LogDate = x.DateLogged,
                LogDescription = x.Log,
                LogKey = x.LogKey,
                Type = (ActivityLogType)x.Type
            }).OrderByDescending(x => x.LogDate).ToList();

            return logs;
        }
    }
}
