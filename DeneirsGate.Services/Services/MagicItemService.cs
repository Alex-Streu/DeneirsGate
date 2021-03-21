using DeneirsGate.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeneirsGate.Services
{
    public class MagicItemService : DeneirsService
    {
        public List<MagicItemViewModel> GetMagicItems(Guid userId, Guid campaignId, bool customOnly = true)
        {
            if (customOnly) { UserHasAccess(userId, campaignId); }

            var items = new List<MagicItemViewModel>();
            using (DBReset())
            {
                if (!customOnly)
                {
                    var _items = DB.MagicItems.ToList();
                    foreach (var item in _items)
                    {
                        var rarity = DB.MagicItemRarities.Where(x => x.RarityKey == item.Rarity).FirstOrDefault();
                        items.Add(new MagicItemViewModel
                        {
                            Description = item.Description,
                            HasAttunement = item.HasAttunement,
                            ItemKey = item.ItemKey,
                            Name = item.Name,
                            Rarity = rarity.Name,
                            RarityValue = rarity.Rarity,
                            Type = DB.MagicItemTypes.Where(x => x.TypeKey == item.Type).Select(x => x.Name).FirstOrDefault()
                        });
                    }
                }
            }

            return items.OrderBy(x => x.Name).ToList();
        }

        public MagicItemViewModel GetMagicItem(Guid userId, Guid itemId)
        {
            var item = new MagicItemViewModel();
            using (DBReset())
            {
                var _item = DB.MagicItems.FirstOrDefault(x => x.ItemKey == itemId);
                var rarity = DB.MagicItemRarities.FirstOrDefault(x => x.RarityKey == _item.Rarity);
                item = new MagicItemViewModel
                {
                    Description = _item.Description,
                    HasAttunement = _item.HasAttunement,
                    ItemKey = _item.ItemKey,
                    Name = _item.Name,
                    Rarity = rarity.Name,
                    RarityValue = rarity.Rarity,
                    Type = DB.MagicItemTypes.Where(x => x.TypeKey == _item.Type).Select(x => x.Name).FirstOrDefault()
                };
            }

            return item;
        }

        public MagicItemEditModel GetEditMagicItem(Guid userId, Guid itemId, Guid campaignId)
        {
            var item = CreateItem(campaignId, itemId);
            if (item == null)
            {
                using (DBReset())
                {
                    item = DB.MagicItems.Where(x => x.ItemKey == itemId).Select(x => new MagicItemEditModel
                    {
                        ItemKey = x.ItemKey,
                        HasAttunement = x.HasAttunement,
                        Rarity = x.Rarity,
                        CampaignKey = campaignId,
                        Description = x.Description,
                        Name = x.Name,
                        Type = x.Type
                    }).FirstOrDefault();
                }
            }

            return item;
        }

        MagicItemEditModel CreateItem(Guid campaignId, Guid itemId)
        {
            var exists = false;
            using (DBReset())
            {
                if (DB.MagicItems.FirstOrDefault(x => x.ItemKey == itemId) != null)
                {
                    exists = true;
                }
            }

            if (exists) { return null; }

            return new MagicItemEditModel
            {
                CampaignKey = campaignId,
                ItemKey = itemId
            };
        }

        public void Update(Guid userId, MagicItemPostModel model)
        {
            using (DBReset())
            {
                bool add = false;

                var item = DB.MagicItems.FirstOrDefault(x => x.ItemKey == model.ItemKey);
                if (item == null)
                {
                    add = true;
                    item = new MagicItem
                    {
                        ItemKey = model.ItemKey
                    };
                }
                
                item.Description = model.Description;
                item.Name = model.Name;
                item.Type = model.Type;
                item.HasAttunement = model.HasAttunement;
                item.Rarity = model.Rarity;

                if (add)
                {
                    DB.MagicItems.Add(item);
                }

                DB.SaveChanges();
            }
        }

        public void Delete(Guid userId, Guid itemId, bool isAdmin = false)
        {
            //Add user access for custom

            using (DBReset())
            {
                var item = DB.MagicItems.FirstOrDefault(x => x.ItemKey == itemId);

                if (isAdmin)
                {
                    DB.MagicItems.Remove(item);
                }

                DB.SaveChanges();
            }
        }

        public List<MagicItemRarityViewModel> GetRarities()
        {
            var rarities = new List<MagicItemRarityViewModel>();
            using (DBReset())
            {
                rarities = DB.MagicItemRarities.OrderBy(x => x.Rarity).Select(x => new MagicItemRarityViewModel
                {
                    Name = x.Name,
                    RarityKey = x.RarityKey
                }).ToList();
            }

            return rarities;
        }

        public List<MagicItemTypeViewModel> GetTypes()
        {
            var types = new List<MagicItemTypeViewModel>();
            using (DBReset())
            {
                types = DB.MagicItemTypes.OrderBy(x => x.Name).Select(x => new MagicItemTypeViewModel
                {
                    Name = x.Name,
                    TypeKey = x.TypeKey
                }).ToList();
            }

            return types;
        }

        public List<MagicItemAttunementViewModel> GetAttunements()
        {
            var attunements = new List<MagicItemAttunementViewModel>();

            attunements.Add(new MagicItemAttunementViewModel
            {
                Attunement = true,
                Name = "Requires"
            });

            attunements.Add(new MagicItemAttunementViewModel
            {
                Attunement = false,
                Name = "Doesn't Require"
            });

            return attunements;
        }

        public void GetEncounterItems(Guid userId, EncounterViewModel model)
        {
            using (DBReset())
            {
                var items = new List<MagicItemViewModel>();
                foreach (var item in model.Items)
                {
                    items.Add(GetMagicItem(userId, item.ItemKey));
                }

                model.Items = items;
            }
        }

        public void UploadMagicItem(Guid userId, string name, string description, string rarity, string type, string attunement)
        {
            DBReset();

            // Do not upload monster if it already exists by name
            if (DB.MagicItems.Where(x => x.Name == name).FirstOrDefault() != null) { return; }

            var rarityKey = DB.MagicItemRarities.Where(x => x.Name.ToLower() == rarity.ToLower()).Select(x => x.RarityKey).FirstOrDefault();
            var typeKey = DB.MagicItemTypes.Where(x => x.Name.ToLower() == type.ToLower()).Select(x => x.TypeKey).FirstOrDefault();
            var requiresAttunement = String.IsNullOrEmpty(attunement) ? false : true;

            var magicItem = new MagicItemPostModel
            {
                Description = description,
                HasAttunement = requiresAttunement,
                IsCustom = false,
                ItemKey = Guid.NewGuid(),
                Name = name,
                Rarity = rarityKey,
                Type = typeKey
            };

            Update(userId, magicItem);
        }
    }
}
