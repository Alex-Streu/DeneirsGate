﻿using DeneirsGate.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeneirsGate.Services
{
    public class MonsterService : DeneirsService
    {
        void UserHasMonsterAccess(bool isAdmin, Guid userKey, Guid monsterKey)
        {
            DBReset();

            if (isAdmin) { return; }
            if (DB.UserMonsters.FirstOrDefault(x => x.UserKey == userKey && x.MonsterKey == monsterKey) != null) { return; }
            if (DB.UserMonsters.FirstOrDefault(x => x.MonsterKey == monsterKey) == null) { return; }

            throw new Exception("You do not have access to this content!");
        }

        public List<MonsterViewModel> GetMonsters(Guid userId, Guid campaignId, bool customOnly = true)
        {
            var monsters = new List<MonsterViewModel>();
            var _monsters = new List<Guid>();

            DBReset();

            if (!customOnly) { _monsters = DB.UserMonsters.Where(x => x.IsPublic || x.UserKey == userId).Select(x => x.MonsterKey).ToList(); }
            else { _monsters = DB.UserMonsters.Where(x => x.UserKey == userId).Select(x => x.MonsterKey).ToList(); }

            foreach (var monster in _monsters)
            {
                monsters.Add(GetMonster(userId, monster));
            }

            return monsters.OrderBy(x => x.Name).ToList();
        }

        public MonsterViewModel GetMonster(Guid userId, Guid monsterId)
        {
            var monster = new MonsterViewModel();
            DBReset();

            var _monster = DB.Monsters.FirstOrDefault(x => x.MonsterKey == monsterId);
            var cr = DB.MonsterChallengeRatings.FirstOrDefault(x => x.RatingKey == _monster.ChallengeRating);
            var environmentKeys = DB.MonsterEnvironmentLinkers.Where(x => x.MonsterKey == _monster.MonsterKey).Select(x => x.EnvironmentKey).ToList();
            monster = new MonsterViewModel
            {
                Alignment = _monster.Alignment,
                ChallengeRating = cr.Challenge,
                Description = _monster.Description,
                Difficulty = cr.Difficulty,
                Environments = DB.Environments.Where(x => environmentKeys.Contains(x.EnvironmentKey)).Select(x => x.Name).ToList(),
                MonsterKey = _monster.MonsterKey,
                Name = _monster.Name,
                Size = DB.MonsterSizes.Where(x => x.SizeKey == _monster.Size).Select(x => x.Name).FirstOrDefault(),
                Speed = _monster.Speed,
                Type = DB.MonsterTypes.Where(x => x.TypeKey == _monster.Type).Select(x => x.Name).FirstOrDefault(),
                XP = cr.XP
            };

            return monster;
        }

        public MonsterEditModel GetEditMonster(Guid userId, Guid monsterId, Guid campaignId, bool isAdmin = false)
        {
            UserHasMonsterAccess(isAdmin, userId, monsterId);

            var monster = CreateMonster(campaignId, monsterId);
            if (monster == null)
            {
                DBReset();
                monster = DB.Monsters.Where(x => x.MonsterKey == monsterId).Select(x => new MonsterEditModel
                {
                    Alignment = x.Alignment,
                    CampaignKey = campaignId,
                    ChallengeRating = x.ChallengeRating,
                    Description = x.Description,
                    Environments = DB.MonsterEnvironmentLinkers.Where(y => y.MonsterKey == monsterId).Select(y => y.EnvironmentKey).ToList(),
                    MonsterKey = x.MonsterKey,
                    Name = x.Name,
                    Size = x.Size,
                    Speed = x.Speed,
                    Type = x.Type
                }).FirstOrDefault();
            }

            return monster;
        }

        public MonsterEditModel CreateMonster(Guid campaignId, Guid monsterId)
        {
            var exists = false;
            using (DBReset())
            {
                if (DB.Monsters.FirstOrDefault(x => x.MonsterKey == monsterId) != null)
                {
                    exists = true;
                }
            }

            if (exists) { return null; }

            return new MonsterEditModel
            {
                CampaignKey = campaignId,
                MonsterKey = monsterId,
                Environments = new List<Guid>()
            };
        }

        public void UpdateMonster(Guid userId, MonsterPostModel model, bool isAdmin = false, bool isPublic = false)
        {
            UserHasMonsterAccess(isAdmin, userId, model.MonsterKey);

            DBReset();

            bool add = false;

            var monster = DB.Monsters.FirstOrDefault(x => x.MonsterKey == model.MonsterKey);
            if (monster == null)
            {
                add = true;
                monster = new Monster
                {
                    MonsterKey = model.MonsterKey
                };
            }

            monster.Alignment = model.Alignment;
            monster.ChallengeRating = model.ChallengeRating;
            monster.Description = model.Description;
            monster.Name = model.Name;
            monster.Size = model.Size;
            monster.Speed = model.Speed;
            monster.Type = model.Type;

            //Environments
            DB.MonsterEnvironmentLinkers.RemoveRange(x => x.MonsterKey == monster.MonsterKey);
            foreach (var environment in model.Environments)
            {
                DB.MonsterEnvironmentLinkers.Add(new MonsterEnvironmentLinker
                {
                    EnvironmentKey = environment,
                    MonsterKey = model.MonsterKey
                });
            }

            if (add)
            {
                DB.Monsters.Add(monster);
                DB.UserMonsters.Add(new UserMonster
                {
                    UserKey = userId,
                    MonsterKey = monster.MonsterKey,
                    IsPublic = isPublic
                });
            }

            DB.SaveChanges();
        }

        public void UploadMonster(Guid userId, string name, string description, string size, string type, string alignment, string speed, string cr)
        {
            DBReset();

            if (name.Contains(","))
            {
                var nameList = name.Split(',');
                var tempName = "";
                for (var i = nameList.Count() - 1; i >= 0; i--)
                {
                    tempName += nameList[i].Trim() + " ";
                }
                name = tempName.Trim();
            }

            // Do not upload monster if it already exists by name
            if (DB.Monsters.Where(x => x.Name == name).FirstOrDefault() != null) { return; }

            var sizeKey = DB.MonsterSizes.Where(x => x.Name == size).Select(x => x.SizeKey).FirstOrDefault();
            var typeKey = DB.MonsterTypes.Where(x => x.Name == type).Select(x => x.TypeKey).FirstOrDefault();
            var strCR = "";
            var floatCR = 0f;
            float.TryParse(cr, out floatCR);
            if (floatCR < 1f && floatCR > 0f)
            {
                var div = 1f / floatCR;
                strCR = $"1/{div}";
            }
            else { strCR = cr; }
            var crKey = DB.MonsterChallengeRatings.Where(x => x.Challenge == strCR).Select(x => x.RatingKey).FirstOrDefault();

            speed = speed.Replace("0", "0ft");
            speed = speed.Replace("50", "~");
            speed = speed.Replace("5", "5ft");
            speed = speed.Replace("~", "50");

            var monster = new MonsterPostModel
            {
                Alignment = alignment,
                ChallengeRating = crKey,
                MonsterKey = Guid.NewGuid(),
                Name = name,
                Size = sizeKey,
                Speed = speed,
                Type = typeKey
            };

            UpdateMonster(userId, monster, true, true);
        }

        public void UploadMonsterEnvironments(string monster, string environment)
        {
            DBReset();

            var envKey = DB.Environments.Where(x => x.Name.ToLower() == environment.ToLower()).Select(x => x.EnvironmentKey).FirstOrDefault();
            var monsterKey = DB.Monsters.Where(x => x.Name.ToLower() == monster.ToLower()).Select(x => x.MonsterKey).FirstOrDefault();

            if (envKey != Guid.Empty && monsterKey != Guid.Empty && DB.MonsterEnvironmentLinkers.FirstOrDefault(x => x.MonsterKey == monsterKey && x.EnvironmentKey == envKey) == null)
            {
                DB.MonsterEnvironmentLinkers.Add(new MonsterEnvironmentLinker
                {
                    MonsterKey = monsterKey,
                    EnvironmentKey = envKey
                });

                DB.SaveChanges();
            }
        }

        public void Delete(Guid userId, Guid monsterId, bool isAdmin = false)
        {
            DBReset();
            var access = DB.UserMonsters.FirstOrDefault(x => x.MonsterKey == monsterId);
            var monster = DB.Monsters.FirstOrDefault(x => x.MonsterKey == monsterId);

            if (isAdmin || userId == access.UserKey)
            {
                DB.Monsters.Remove(monster);
                DB.MonsterEnvironmentLinkers.RemoveRange(x => x.MonsterKey == monster.MonsterKey);
                DB.UserMonsters.RemoveRange(x => x.MonsterKey == monsterId);
            }

            DB.SaveChanges();
        }

        public List<MonsterSizeViewModel> GetSizes()
        {
            var sizes = new List<MonsterSizeViewModel>();
            using (DBReset())
            {
                sizes = DB.MonsterSizes.OrderBy(x => x.SortOrder).Select(x => new MonsterSizeViewModel
                {
                    Name = x.Name,
                    SizeKey = x.SizeKey
                }).ToList();
            }

            return sizes;
        }

        public List<MonsterTypeViewModel> GetTypes()
        {
            var sizes = new List<MonsterTypeViewModel>();
            using (DBReset())
            {
                sizes = DB.MonsterTypes.OrderBy(x => x.Name).Select(x => new MonsterTypeViewModel
                {
                    Name = x.Name,
                    TypeKey = x.TypeKey
                }).ToList();
            }

            return sizes;
        }

        public List<MonsterChallengeRatingViewModel> GetChallengeRatings()
        {
            var ratings = new List<MonsterChallengeRatingViewModel>();
            using (DBReset())
            {
                ratings = DB.MonsterChallengeRatings.OrderBy(x => x.Difficulty).Select(x => new MonsterChallengeRatingViewModel
                {
                    Challenge = x.Challenge,
                    Difficulty = x.Difficulty,
                    Proficiency = x.Proficiency,
                    RatingKey = x.RatingKey,
                    XP = x.XP
                }).ToList();
            }

            return ratings;
        }

        public void GetEncounterMonsters(Guid userId, EncounterViewModel model)
        {
            DBReset();
            var totalXp = 0;
            foreach (var monster in model.Monsters)
            {
                var _monster = GetMonster(userId, monster.MonsterKey);
                var monsterItem = model.Monsters.FirstOrDefault(x => x.MonsterKey == monster.MonsterKey);
                monsterItem.Alignment = _monster.Alignment;
                monsterItem.ChallengeRating = _monster.ChallengeRating;
                monsterItem.Description = _monster.Description;
                monsterItem.Difficulty = _monster.Difficulty;
                monsterItem.Environments = _monster.Environments;
                monsterItem.MonsterKey = _monster.MonsterKey;
                monsterItem.Name = _monster.Name;
                monsterItem.Size = _monster.Size;
                monsterItem.Speed = _monster.Speed;
                monsterItem.Type = _monster.Type;
                monsterItem.XP = _monster.XP;
                monsterItem.Count = monster.Count;

                totalXp += monsterItem.XP * monster.Count;
            }

            model.TotalXP = totalXp;
        }
    }
}
