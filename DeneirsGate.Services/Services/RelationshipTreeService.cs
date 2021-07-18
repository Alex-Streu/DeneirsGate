using DeneirsGate.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeneirsGate.Services
{
    public class RelationshipTreeService : DeneirsService
    {
        public RelationshipTreeService(DataEntities _db)
        {
            db = _db;
        }

        public List<RelationshipTreeSearchModel> GetCharacterTrees(Guid userId, Guid campaignKey, Guid characterId)
        {
            UserHasAccess(userId, campaignKey);

            var trees = new List<RelationshipTreeSearchModel>();

            var treeIds = db.RelationshipTreeCharacters.Where(x => x.CharacterKey == characterId).Select(x => x.TreeKey).ToList();
            trees = db.RelationshipTrees.Where(x => x.CampaignKey == campaignKey && treeIds.Contains(x.TreeKey)).Select(x => new RelationshipTreeSearchModel
            {
                TreeKey = x.TreeKey,
                Name = x.Name
            }).ToList();

            foreach (var tree in trees)
            {
                var shallowNames = db.RelationshipTreeCharacters.Where(x => x.TreeKey == tree.TreeKey && x.IsShallow).Select(x => x.Name).ToList();
                var charIds = db.RelationshipTreeCharacters.Where(x => x.TreeKey == tree.TreeKey && !x.IsShallow).Select(x => x.CharacterKey).ToList();
                var charNames = db.Characters.Where(x => charIds.Contains(x.CharacterKey)).Select(x => x.FirstName + " " + x.LastName).ToList();
                charNames.AddRange(shallowNames);

                tree.CharacterList = String.Join(",", charNames);
            }

            return trees;
        }

        public List<RelationshipTreeSearchModel> GetSearchTrees(Guid userId, Guid campaignKey)
        {
            UserHasAccess(userId, campaignKey);

            var trees = db.RelationshipTrees.Where(x => x.CampaignKey == campaignKey).Select(x => new RelationshipTreeSearchModel
            {
                TreeKey = x.TreeKey,
                Name = x.Name
            }).ToList();

            foreach (var tree in trees)
            {
                var shallowNames = db.RelationshipTreeCharacters.Where(x => x.TreeKey == tree.TreeKey && x.IsShallow).Select(x => x.Name).ToList();
                var charIds = db.RelationshipTreeCharacters.Where(x => x.TreeKey == tree.TreeKey && !x.IsShallow).Select(x => x.CharacterKey).ToList();
                var charNames = db.Characters.Where(x => charIds.Contains(x.CharacterKey)).Select(x => x.FirstName + " " + x.LastName).ToList();
                charNames.AddRange(shallowNames);

                tree.CharacterList = String.Join(",", charNames);
            }

            return trees;
        }

        public RelationshipTreeViewModel GetRelationshipTree(Guid id, Guid campaignKey)
        {
            var tree = new RelationshipTreeViewModel();

            tree = db.RelationshipTrees.Where(x => x.TreeKey == id).Select(x => new RelationshipTreeViewModel
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
            var tiers = db.RelationshipTreeTiers.Where(x => x.TreeKey == id).OrderBy(x => x.SortOrder).ToList();
            foreach (var tier in tiers)
            {
                var newTier = new RelationshipTreeTierViewModel
                {
                    TierKey = tier.TierKey,
                    SortOrder = tier.SortOrder,
                    Characters = new List<RelationshipTreeCharacterViewModel>()
                };

                var characters = db.RelationshipTreeCharacters.Where(x => x.TreeKey == id && x.TierKey == tier.TierKey).ToList();
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
                        var charInfo = db.Characters.FirstOrDefault(x => x.CharacterKey == character.CharacterKey);
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
            var ignoreKeys = db.RelationshipTreeCharacters.Where(x => x.TreeKey == treeKey && x.IsShallow == false).Select(x => x.CharacterKey);
            var includeKeys = db.CampaignCharacterLinkers.Where(x => x.CampaignKey == campaignKey).Select(x => x.CharacterKey).ToList();

            return db.Characters.Where(x => includeKeys.Contains(x.CharacterKey) && !ignoreKeys.Contains(x.CharacterKey)).Select(x => new RelationshipTreeCharacterViewModel
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

            var add = false;
            var tree = db.RelationshipTrees.FirstOrDefault(x => x.TreeKey == model.TreeKey);

            if (tree == null)
            {
                tree = new RelationshipTree();
                tree.TreeKey = model.TreeKey;
                tree.CampaignKey = model.CampaignKey;
                add = true;
            }

            tree.Name = model.Name;

            //Tiers
            db.RelationshipTreeTiers.RemoveRange(db.RelationshipTreeTiers.Where(x => x.TreeKey == model.TreeKey).ToList());
            foreach (var item in model.Tiers)
            {
                item.TierKey = item.TierKey == Guid.Empty ? Guid.NewGuid() : item.TierKey;
                db.RelationshipTreeTiers.Add(new RelationshipTreeTier
                {
                    TreeKey = model.TreeKey,
                    TierKey = item.TierKey,
                    SortOrder = item.SortOrder
                });
            }

            //Characters
            db.RelationshipTreeCharacters.RemoveRange(db.RelationshipTreeCharacters.Where(x => x.TreeKey == model.TreeKey).ToList());
            foreach (var tier in model.Tiers)
            {
                foreach (var item in tier.Characters)
                {
                    db.RelationshipTreeCharacters.Add(new RelationshipTreeCharacter
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
                db.RelationshipTrees.Add(tree);
            }

            db.SaveChanges();
        }

        public void DeleteRelationshipTree(Guid userKey, Guid treeId)
        {
            db.RelationshipTrees.RemoveRange(x => x.TreeKey == treeId);
            db.RelationshipTreeTiers.RemoveRange(x => x.TreeKey == treeId);
            db.RelationshipTreeCharacters.RemoveRange(x => x.TreeKey == treeId);

            db.SaveChanges();
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
