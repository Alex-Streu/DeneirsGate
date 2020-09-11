using DeneirsGate.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeneirsGate.Services
{
    public class MonsterService : DeneirsService
    {
        public List<MonsterViewModel> GetMonsters(Guid userId, Guid campaignId, bool customOnly = true)
        {
            if (customOnly) { UserHasAccess(userId, campaignId); }

            var monsters = new List<MonsterViewModel>();
            using (DBReset())
            {
                if (!customOnly)
                {
                    var _monsters = DB.Monsters.ToList();
                    foreach (var monster in _monsters)
                    {
                        var cr = DB.MonsterChallengeRatings.FirstOrDefault(x => x.RatingKey == monster.ChallengeRating);
                        var environmentKeys = DB.MonsterEnvironmentLinkers.Where(x => x.MonsterKey == monster.MonsterKey).Select(x => x.EnvironmentKey).ToList();
                        monsters.Add(new MonsterViewModel
                        {
                            Alignment = monster.Alignment,
                            ChallengeRating = cr.Challenge,
                            Description = monster.Description,
                            Difficulty = cr.Difficulty,
                            Environments = DB.Environments.Where(x => environmentKeys.Contains(x.EnvironmentKey)).Select(x => x.Name).ToList(),
                            MonsterKey = monster.MonsterKey,
                            Name = monster.Name,
                            Size = DB.MonsterSizes.Where(x => x.SizeKey == monster.Size).Select(x => x.Name).FirstOrDefault(),
                            Speed = monster.Speed,
                            Type = DB.MonsterTypes.Where(x => x.TypeKey == monster.Type).Select(x => x.Name).FirstOrDefault()
                        });
                    }
                }
            }

            return monsters;
        }

        public MonsterViewModel GetMonster(Guid userId, Guid monsterId)
        {
            var monster = new MonsterViewModel();
            using (DBReset())
            {
                var _monster = DB.Monsters.FirstOrDefault(x => x.MonsterKey == monsterId);
                var cr = DB.MonsterChallengeRatings.FirstOrDefault(x => x.RatingKey == _monster.ChallengeRating);
                var environmentKeys = DB.MonsterEnvironmentLinkers.Where(x => x.MonsterKey == _monster.MonsterKey).Select(x => x.EnvironmentKey).ToList();
                monster = new MonsterViewModel
                {
                    Alignment = monster.Alignment,
                    ChallengeRating = cr.Challenge,
                    Description = monster.Description,
                    Difficulty = cr.Difficulty,
                    Environments = DB.Environments.Where(x => environmentKeys.Contains(x.EnvironmentKey)).Select(x => x.Name).ToList(),
                    MonsterKey = monster.MonsterKey,
                    Name = monster.Name,
                    Size = DB.MonsterSizes.Where(x => x.SizeKey == _monster.Size).Select(x => x.Name).FirstOrDefault(),
                    Speed = monster.Speed,
                    Type = DB.MonsterTypes.Where(x => x.TypeKey == _monster.Type).Select(x => x.Name).FirstOrDefault()
                };
            }

            return monster;
        }

        public MonsterEditModel GetEditMonster(Guid userId, Guid monsterId, Guid campaignId)
        {
            var monster = CreateMonster(campaignId, monsterId);
            if (monster == null)
            {
                using (DBReset())
                {
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

        public void UpdateMonster(Guid userId, MonsterPostModel model)
        {
            using (DBReset())
            {
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
                }

                DB.SaveChanges();
            }
        }

        public void Delete(Guid userId, Guid monsterId, bool isAdmin = false)
        {
            //Add user access for custom

            using (DBReset())
            {
                var monster = DB.Monsters.FirstOrDefault(x => x.MonsterKey == monsterId);

                if (isAdmin)
                {
                    DB.Monsters.Remove(monster);
                    DB.MonsterEnvironmentLinkers.RemoveRange(x => x.MonsterKey == monster.MonsterKey);
                }

                DB.SaveChanges();
            }
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
    }
}
