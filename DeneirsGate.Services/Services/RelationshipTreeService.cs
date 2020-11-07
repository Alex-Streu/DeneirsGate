using DeneirsGate.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeneirsGate.Services
{
    public class RelationshipTreeService : DeneirsService
    {
        public List<RelationshipTreeSearchModel> GetCharacterTrees(Guid userId, Guid campaignKey, Guid characterId)
        {
            UserHasAccess(userId, campaignKey);

            var trees = new List<RelationshipTreeSearchModel>();

            using (DBReset())
            {
                var treeIds = DB.RelationshipTreeCharacters.Where(x => x.CharacterKey == characterId).Select(x => x.TreeKey).ToList();
                trees = DB.RelationshipTrees.Where(x => x.CampaignKey == campaignKey && treeIds.Contains(x.TreeKey)).Select(x => new RelationshipTreeSearchModel
                {
                    TreeKey = x.TreeKey,
                    Name = x.Name
                }).ToList();

                foreach (var tree in trees)
                {
                    var shallowNames = DB.RelationshipTreeCharacters.Where(x => x.TreeKey == tree.TreeKey && x.IsShallow).Select(x => x.Name).ToList();
                    var charIds = DB.RelationshipTreeCharacters.Where(x => x.TreeKey == tree.TreeKey && !x.IsShallow).Select(x => x.CharacterKey).ToList();
                    var charNames = DB.Characters.Where(x => charIds.Contains(x.CharacterKey)).Select(x => x.FirstName + " " + x.LastName).ToList();
                    charNames.AddRange(shallowNames);

                    tree.CharacterList = String.Join(",", charNames);
                }
            }

            return trees;
        }

        public List<RelationshipTreeSearchModel> GetSearchTrees(Guid userId, Guid campaignKey)
        {
            UserHasAccess(userId, campaignKey);

            var trees = new List<RelationshipTreeSearchModel>();

            using (DBReset())
            {
                trees = DB.RelationshipTrees.Where(x => x.CampaignKey == campaignKey).Select(x => new RelationshipTreeSearchModel
                {
                    TreeKey = x.TreeKey,
                    Name = x.Name
                }).ToList();

                foreach (var tree in trees)
                {
                    var shallowNames = DB.RelationshipTreeCharacters.Where(x => x.TreeKey == tree.TreeKey && x.IsShallow).Select(x => x.Name).ToList();
                    var charIds = DB.RelationshipTreeCharacters.Where(x => x.TreeKey == tree.TreeKey && !x.IsShallow).Select(x => x.CharacterKey).ToList();
                    var charNames = DB.Characters.Where(x => charIds.Contains(x.CharacterKey)).Select(x => x.FirstName + " " + x.LastName).ToList();
                    charNames.AddRange(shallowNames);

                    tree.CharacterList = String.Join(",", charNames);
                }
            }

            return trees;
        }

        public RelationshipTreeViewModel GetRelationshipTree(Guid id, Guid campaignKey)
        {
            var tree = new RelationshipTreeViewModel();

            tree = DB.RelationshipTrees.Where(x => x.TreeKey == id).Select(x => new RelationshipTreeViewModel
            {
                TreeKey = x.TreeKey,
                CampaignKey = x.CampaignKey,
                Name = x.Name
            }).FirstOrDefault();

            if (tree == null)
            {
                tree = new RelationshipTreeViewModel
                {
                    Tiers = new List<RelationshipTreeTierViewModel>(),
                    TreeKey = id,
                    CampaignKey = campaignKey,
                    AvailableCharacters = GetAvailableCharacters(id, campaignKey)
            };

                return tree;
            }

            tree.Tiers = new List<RelationshipTreeTierViewModel>();
            var tiers = DB.RelationshipTreeTiers.Where(x => x.TreeKey == id).OrderBy(x => x.SortOrder).ToList();
            foreach (var tier in tiers)
            {
                var newTier = new RelationshipTreeTierViewModel
                {
                    TierKey = tier.TierKey,
                    SortOrder = tier.SortOrder,
                    Characters = new List<RelationshipTreeCharacterViewModel>()
                };

                var characters = DB.RelationshipTreeCharacters.Where(x => x.TreeKey == id && x.TierKey == tier.TierKey).ToList();
                foreach (var character in characters)
                {
                    if (character.IsShallow)
                    {
                        newTier.Characters.Add(new RelationshipTreeCharacterViewModel
                        {
                            CharacterKey = character.CharacterKey,
                            Backstory = character.Backstory,
                            Name = character.Name,
                            IsShallow = true
                        });
                    }
                    else
                    {
                        var charInfo = DB.Characters.FirstOrDefault(x => x.CharacterKey == character.CharacterKey);
                        newTier.Characters.Add(new RelationshipTreeCharacterViewModel
                        {
                            CharacterKey = character.CharacterKey,
                            Backstory = charInfo.Backstory,
                            Name = (charInfo.FirstName + " " + charInfo.LastName).Trim(),
                            Portrait = charInfo.Portrait,
                            IsShallow = false
                        });
                    }
                }

                tree.Tiers.Add(newTier);
            }

            tree.AvailableCharacters = GetAvailableCharacters(id, campaignKey);

            return tree;
        }

        public List<RelationshipTreeCharacterViewModel> GetAvailableCharacters(Guid treeKey, Guid campaignKey)
        {
            var ignoreKeys = DB.RelationshipTreeCharacters.Where(x => x.TreeKey == treeKey && x.IsShallow == false).Select(x => x.CharacterKey);
            var includeKeys = DB.CampaignCharacterLinkers.Where(x => x.CampaignKey == campaignKey).Select(x => x.CharacterKey).ToList();

            return DB.Characters.Where(x => includeKeys.Contains(x.CharacterKey) && !ignoreKeys.Contains(x.CharacterKey)).Select(x => new RelationshipTreeCharacterViewModel
            {
                Backstory = x.Backstory,
                CharacterKey = x.CharacterKey,
                IsShallow = false,
                Name = (x.FirstName + " " + x.LastName).Trim(),
                Portrait = x.Portrait
            }).ToList();
        }

        public void UpdateRelationshipTree(Guid userKey, RelationshipTreePostModel model)
        {
            UserHasAccess(userKey, model.CampaignKey);

            using (DBReset())
            {
                var add = false;
                var tree = DB.RelationshipTrees.FirstOrDefault(x => x.TreeKey == model.TreeKey);

                if (tree == null)
                {
                    tree = new RelationshipTree();
                    tree.TreeKey = model.TreeKey;
                    tree.CampaignKey = model.CampaignKey;
                    add = true;
                }

                tree.Name = model.Name;

                //Tiers
                DB.RelationshipTreeTiers.RemoveRange(DB.RelationshipTreeTiers.Where(x => x.TreeKey == model.TreeKey).ToList());
                foreach (var item in model.Tiers)
                {
                    item.TierKey = item.TierKey == Guid.Empty ? Guid.NewGuid() : item.TierKey;
                    DB.RelationshipTreeTiers.Add(new RelationshipTreeTier
                    {
                        TreeKey = model.TreeKey,
                        TierKey = item.TierKey,
                        SortOrder = item.SortOrder
                    });
                }

                //Characters
                DB.RelationshipTreeCharacters.RemoveRange(DB.RelationshipTreeCharacters.Where(x => x.TreeKey == model.TreeKey).ToList());
                foreach (var tier in model.Tiers)
                {
                    foreach (var item in tier.Characters)
                    {
                        DB.RelationshipTreeCharacters.Add(new RelationshipTreeCharacter
                        {
                            TreeKey = model.TreeKey,
                            TierKey = tier.TierKey,
                            CharacterKey = item.CharacterKey == Guid.Empty ? Guid.NewGuid() : item.CharacterKey,
                            IsShallow = item.IsShallow,
                            Backstory = item.Backstory,
                            Name = item.Name
                        });
                    }
                }

                if (add)
                {
                    DB.RelationshipTrees.Add(tree);
                }

                DB.SaveChanges();
            }
        }

        public void DeleteRelationshipTree(Guid userKey, Guid treeId)
        {
            using (DBReset())
            {
                DB.RelationshipTrees.RemoveRange(x => x.TreeKey == treeId);
                DB.RelationshipTreeTiers.RemoveRange(x => x.TreeKey == treeId);
                DB.RelationshipTreeCharacters.RemoveRange(x => x.TreeKey == treeId);

                DB.SaveChanges();
            }
        }

        public Dictionary<string, string> GetSearchDropdown()
        {
            var options = new Dictionary<string, string>
            {
                { "Character", "By Character" },
                { "Name", "By Name" }
            };

            return options;
        }
    }
}
