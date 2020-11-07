using DeneirsGate.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeneirsGate.Services
{
    public class DungeonService : DeneirsService
    {
        enum ContentType
        {
            Dungeon, Trap
        }

        void UserHasDungeonAccess(Guid campaignId, Guid contentId, ContentType type)
        {
            var hasAccess = false;

            using (DBReset())
            {
                switch (type)
                {
                    case ContentType.Dungeon:
                        if (DB.CampaignDungeonLinkers.FirstOrDefault(x => x.CampaignKey == campaignId && x.DungeonKey == contentId) != null
                            || DB.CampaignDungeonLinkers.FirstOrDefault(x => x.DungeonKey == contentId) == null)
                        {
                            hasAccess = true;
                        }
                        break;
                    case ContentType.Trap:
                        if (DB.CampaignTrapLinkers.FirstOrDefault(x => x.CampaignKey == campaignId && x.TrapKey == contentId) != null
                            || DB.CampaignTrapLinkers.FirstOrDefault(x => x.TrapKey == contentId) == null)
                        {
                            hasAccess = true;
                        }
                        break;
                }
            }

            if (!hasAccess)
            {
                throw new Exception("You do not have access to this content!");
            }
        }

        #region Dungeons
        public List<DungeonListViewModel> GetDungeons(Guid userId, Guid campaignId)
        {
            UserHasAccess(userId, campaignId);

            var dungeons = new List<DungeonListViewModel>();
            using (DBReset())
            {
                var dungeonIds = DB.CampaignDungeonLinkers.Where(x => x.CampaignKey == campaignId).Select(x => x.DungeonKey).ToList();
                var dbDungeons = DB.Dungeons.Where(x => dungeonIds.Contains(x.DungeonKey)).ToList();
                foreach (var item in dbDungeons)
                {
                    dungeons.Add(new DungeonListViewModel
                    {
                        DungeonKey = item.DungeonKey,
                        Name = item.Name,
                        Description = item.Description,
                        Rows = DB.DungeonTiles.Where(y => y.DungeonKey == item.DungeonKey).Select(y => y.Row).DefaultIfEmpty(4).Max() + 1,
                        Columns = DB.DungeonTiles.Where(y => y.DungeonKey == item.DungeonKey).Select(y => y.Column).DefaultIfEmpty(4).Max() + 1,
                    });
                }
            }

            return dungeons;
        }

        public DungeonViewModel GetDungeon(Guid userId, Guid campaignId, Guid dungeonId)
        {
            UserHasAccess(userId, campaignId);
            UserHasDungeonAccess(campaignId, dungeonId, ContentType.Dungeon);

            var dungeon = CreateDungeon(dungeonId);
            if (dungeon == null)
            {
                using (DBReset())
                {
                    dungeon = DB.Dungeons.Where(x => x.DungeonKey == dungeonId).Select(x => new DungeonViewModel
                    {
                        DungeonKey = x.DungeonKey,
                        Name = x.Name,
                        Description = x.Description,
                        Rows = DB.DungeonTiles.Where(y => y.DungeonKey == dungeonId).Select(y => y.Row).DefaultIfEmpty(4).Max() + 1,
                        Columns = DB.DungeonTiles.Where(y => y.DungeonKey == dungeonId).Select(y => y.Column).DefaultIfEmpty(4).Max() + 1,
                    }).FirstOrDefault();

                    dungeon.Tiles = DB.DungeonTiles.Where(x => x.DungeonKey == dungeonId).Select(x => new DungeonTileViewModel
                    {
                        TileKey = x.TileKey,
                        Column = x.Column,
                        Row = x.Row,
                        Description = x.Description,
                        Image = x.Image,
                        Index = x.Index
                    }).ToList() ?? new List<DungeonTileViewModel>();

                    foreach (var tile in dungeon.Tiles)
                    {
                        tile.Trap = DB.DungeonTileTraps.Where(x => x.TileKey == tile.TileKey).Select(x => new DungeonTileTrapViewModel
                        {
                            TrapKey = x.TrapKey,
                            AttackBonus = x.AttackBonus,
                            Damage = x.Damage,
                            Description = x.Description,
                            Name = x.Name,
                            NatureKey = x.NatureKey,
                            SaveDC = x.SaveDC,
                            TypeKey = x.TypeKey
                        }).FirstOrDefault();

                        tile.Encounter = DB.DungeonTileEncounters.Where(x => x.TileKey == tile.TileKey).Select(x => new EncounterViewModel
                        {
                            EncounterKey = x.EncounterKey
                        }).FirstOrDefault();
                    }
                }
            }

            return dungeon;
        }

        public DungeonViewModel CreateDungeon(Guid dungeonId)
        {
            var exists = false;
            using (DBReset())
            {
                if (DB.Dungeons.FirstOrDefault(x => x.DungeonKey == dungeonId) != null)
                {
                    exists = true;
                }
            }

            if (exists) { return null; }

            return new DungeonViewModel
            {
                DungeonKey = dungeonId,
                Rows = 5,
                Columns = 5,
                Tiles = new List<DungeonTileViewModel>()
            };
        }

        public void UpdateDungeon(Guid userId, Guid campaignId, DungeonPostModel model)
        {
            UserHasAccess(userId, campaignId);
            UserHasDungeonAccess(campaignId, model.DungeonKey, ContentType.Dungeon);

            using (DBReset())
            {
                bool add = false;
                var dungeon = DB.Dungeons.FirstOrDefault(x => x.DungeonKey == model.DungeonKey);
                if (dungeon == null)
                {
                    add = true;
                    dungeon = new Dungeon
                    {
                        DungeonKey = model.DungeonKey
                    };
                }

                dungeon.Name = model.Name;
                dungeon.Description = model.Description;

                //Remove pre-existing tile assets
                var existingTileKeys = DB.DungeonTiles.Where(x => x.DungeonKey == model.DungeonKey).Select(x => x.TileKey).ToList();
                DB.DungeonTiles.RemoveRange(x => x.DungeonKey == model.DungeonKey);
                DB.DungeonTileTraps.RemoveRange(x => existingTileKeys.Contains(x.TileKey));
                DB.DungeonTileEncounters.RemoveRange(x => existingTileKeys.Contains(x.TileKey));

                //Tiles
                foreach (var tile in model.Tiles)
                {
                    var newTile = new DungeonTile
                    {
                        DungeonKey = model.DungeonKey,
                        TileKey = tile.TileKey,
                        Row = tile.Row,
                        Column = tile.Column,
                        Description = tile.Description,
                        Image = tile.Image,
                        Index = tile.Index
                    };

                    DB.DungeonTiles.Add(newTile);

                    //Traps
                    if (tile.Trap != null)
                    {
                        var trap = tile.Trap;
                        DB.DungeonTileTraps.Add(new DungeonTileTrap
                        {
                            TrapKey = Guid.NewGuid(),
                            TileKey = newTile.TileKey,
                            AttackBonus = trap.AttackBonus,
                            Damage = trap.Damage,
                            Description = trap.Description,
                            Name = trap.Name,
                            NatureKey = trap.NatureKey,
                            SaveDC = trap.SaveDC,
                            TypeKey = trap.TypeKey
                        });
                    }

                    //Encounters
                    if (tile.Encounter != null)
                    {
                        DB.DungeonTileEncounters.Add(new DungeonTileEncounter
                        {
                            TileKey = newTile.TileKey,
                            EncounterKey = tile.Encounter.EncounterKey
                        });
                    }
                }

                if (add)
                {
                    DB.Dungeons.Add(dungeon);
                    DB.CampaignDungeonLinkers.Add(new CampaignDungeonLinker
                    {
                        CampaignKey = campaignId,
                        DungeonKey = dungeon.DungeonKey
                    });
                }

                DB.SaveChanges();
            }
        }

        public List<Guid> DeleteDungeon(Guid userId, Guid campaignId, Guid dungeonId)
        {
            UserHasAccess(userId, campaignId);
            UserHasDungeonAccess(campaignId, dungeonId, ContentType.Dungeon);

            var encounterIds = new List<Guid>();
            using (DBReset())
            {
                var tileIds = DB.DungeonTiles.Where(x => x.DungeonKey == dungeonId).Select(x => x.TileKey).ToList() ?? new List<Guid>();
                encounterIds = DB.DungeonTileEncounters.Where(x => tileIds.Contains(x.TileKey)).Select(x => x.EncounterKey).ToList() ?? new List<Guid>();

                DB.Dungeons.RemoveRange(x => x.DungeonKey == dungeonId);
                DB.DungeonTiles.RemoveRange(x => x.DungeonKey == dungeonId);
                DB.DungeonTileTraps.RemoveRange(x => tileIds.Contains(x.TileKey));
                DB.DungeonTileEncounters.RemoveRange(x => tileIds.Contains(x.TileKey));

                DB.CampaignDungeonLinkers.RemoveRange(x => x.DungeonKey == dungeonId);

                DB.SaveChanges();
            }

            return encounterIds;
        }
        #endregion

        #region Traps
        public List<TrapViewModel> GetTraps(Guid userId, Guid campaignId)
        {
            UserHasAccess(userId, campaignId);

            var traps = new List<TrapViewModel>();
            using (DBReset())
            {
                var trapIds = DB.CampaignTrapLinkers.Where(x => x.CampaignKey == campaignId).Select(x => x.TrapKey).ToList();
                foreach (var id in trapIds)
                {
                    traps.Add(GetTrap(userId, campaignId, id));
                }
            }

            return traps;
        }

        public TrapEditModel GetTrapEdit(Guid userId, Guid campaignId, Guid trapId)
        {
            UserHasAccess(userId, campaignId);

            var trap = CreateTrap(campaignId, trapId);
            if (trap == null)
            {
                using (DBReset())
                {
                    trap = DB.Traps.Where(x => x.TrapKey == trapId).Select(x => new TrapEditModel
                    {
                        TrapKey = x.TrapKey,
                        Description = x.Description,
                        Name = x.Name,
                        NatureKey = x.Nature,
                        TypeKey = x.Type
                    }).FirstOrDefault();
                }
            }

            return trap;
        }

        public TrapEditModel CreateTrap(Guid campaignId, Guid trapId)
        {
            var exists = false;
            using (DBReset())
            {
                if (DB.Traps.FirstOrDefault(x => x.TrapKey == trapId) != null)
                {
                    exists = true;
                }
            }

            if (exists) { return null; }

            return new TrapEditModel
            {
                TrapKey = trapId
            };
        }

        public TrapStatsModel GenerateTrapStats(Guid typeId, int partyLevel)
        {
            var stats = new TrapStatsModel();
            using (DBReset())
            {
                var type = DB.TrapTypes.FirstOrDefault(x => x.TypeKey == typeId);

                var attacks = type.AttackBonus.Split('-');
                var saveDCs = type.SaveDC.Split('-');

                var random = new Random();
                stats.SaveDC = random.Next(Int32.Parse(saveDCs[0]), Int32.Parse(saveDCs[1]) + 1);
                stats.AttackBonus = random.Next(Int32.Parse(attacks[0]), Int32.Parse(attacks[1]) + 1);
                stats.Damage = DB.TrapTypeDamages.Where(x => x.TypeKey == typeId && x.MinLevel <= partyLevel && x.MaxLevel >= partyLevel).Select(x => x.Damage).FirstOrDefault();
            }

            return stats;
        }

        public TrapViewModel GetTrap(Guid userId, Guid campaignId, Guid trapId)
        {
            UserHasAccess(userId, campaignId);
            UserHasDungeonAccess(campaignId, trapId, ContentType.Trap);

            var trap = new TrapViewModel();
            using (DBReset())
            {
                var dbTrap = DB.Traps.FirstOrDefault(x => x.TrapKey == trapId);

                trap.TrapKey = dbTrap.TrapKey;
                trap.Description = dbTrap.Description;
                trap.Name = dbTrap.Name;
                trap.Nature = DB.TrapNatures.Where(x => x.NatureKey == dbTrap.Nature).Select(x => x.Name).FirstOrDefault();
                trap.Type = DB.TrapTypes.Where(x => x.TypeKey == dbTrap.Type).Select(x => x.Name).FirstOrDefault();
                trap.NatureKey = dbTrap.Nature;
                trap.TypeKey = dbTrap.Type;
            }

            return trap;
        }

        public void UpdateTrap(Guid userId, Guid campaignId, TrapPostModel model)
        {
            UserHasAccess(userId, campaignId);
            UserHasDungeonAccess(campaignId, model.TrapKey, ContentType.Trap);

            using (DBReset())
            {
                bool add = false;

                var trap = DB.Traps.FirstOrDefault(x => x.TrapKey == model.TrapKey);
                if (trap == null)
                {
                    add = true;
                    trap = new Trap
                    {
                        TrapKey = model.TrapKey
                    };
                }
                
                trap.Description = model.Description;
                trap.Name = model.Name;
                trap.Nature = model.NatureKey;
                trap.Type = model.TypeKey;

                if (add)
                {
                    DB.Traps.Add(trap);
                    DB.CampaignTrapLinkers.Add(new CampaignTrapLinker
                    {
                        CampaignKey = campaignId,
                        TrapKey = model.TrapKey
                    });
                }

                DB.SaveChanges();
            }
        }

        public void DeleteTrap(Guid userId, Guid campaignId, Guid trapId)
        {
            UserHasAccess(userId, campaignId);
            UserHasDungeonAccess(campaignId, trapId, ContentType.Trap);

            using (DBReset())
            {
                DB.Traps.RemoveRange(x => x.TrapKey == trapId);
                DB.CampaignTrapLinkers.RemoveRange(x => x.TrapKey == trapId);

                DB.SaveChanges();
            }
        }

        public TrapViewModel SuggestTrap(Guid userId, Guid campaignId, int partyLevel, string name, Guid? natureKey, Guid? typeKey)
        {
            UserHasAccess(userId, campaignId);

            TrapViewModel trap = null;
            using (DBReset())
            {
                var trapIds = DB.CampaignTrapLinkers.Where(x => x.CampaignKey == campaignId).Select(x => x.TrapKey).ToList();
                var traps = DB.Traps.Where(x => trapIds.Contains(x.TrapKey)
                        && (name == null || x.Name.Contains(name))
                        && (natureKey == null || x.Nature == natureKey.Value)
                        && (typeKey == null || x.Type == typeKey.Value)).ToList();

                if (traps.Count > 0)
                {
                    var random = new Random();
                    var selectTrap = traps[random.Next(traps.Count)];
                    var stats = GenerateTrapStats(selectTrap.Type, partyLevel);

                    using (DBReset())
                    {
                        trap = new TrapViewModel
                        {
                            AttackBonus = stats.AttackBonus,
                            Damage = stats.Damage,
                            Description = selectTrap.Description,
                            Name = selectTrap.Name,
                            Nature = DB.TrapNatures.FirstOrDefault(x => x.NatureKey == selectTrap.Nature).Name,
                            NatureKey = selectTrap.Nature,
                            SaveDC = stats.SaveDC,
                            TrapKey = selectTrap.TrapKey,
                            Type = DB.TrapTypes.FirstOrDefault(x => x.TypeKey == selectTrap.Type).Name,
                            TypeKey = selectTrap.Type
                        };
                    }
                }
            }

            return trap;
        }

        public List<TrapNatureViewModel> GetTrapNatures()
        {
            var list = new List<TrapNatureViewModel>();
            using (DBReset())
            {
                list = DB.TrapNatures.OrderBy(x => x.Name).Select(x => new TrapNatureViewModel
                {
                    Name = x.Name,
                    NatureKey = x.NatureKey
                }).ToList();
            }

            return list;
        }

        public List<TrapTypeViewModel> GetTrapTypes()
        {
            var list = new List<TrapTypeViewModel>();
            using (DBReset())
            {
                list = DB.TrapTypes.OrderByDescending(x => x.Name == "Setback")
                    .ThenByDescending(x => x.Name == "Dangerous")
                    .ThenByDescending(x => x.Name == "Deadly")
                    .Select(x => new TrapTypeViewModel
                {
                    Name = x.Name,
                    TypeKey = x.TypeKey
                }).ToList();
            }

            return list;
        }
        #endregion
    }
}
