using DeneirsGate.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeneirsGate.Services
{
    public class EventService : DeneirsService
    {
        enum XPDifficulty
        {
            Easy, Medium, Hard, Deadly
        };

        enum ItemRarity
        {
            Common = 1, Uncommon, Rare, VeryRare, Legendary, Artifact
        }

        enum CoinType
        {
            Copper, Silver, Electrum, Gold, Platinum
        }

        int[,] _xpThresholds = new int[,]
        {
            { 25, 50, 75, 100 },
            { 50, 100, 150, 200 },
            { 75, 150, 225, 400 },
            { 125, 250, 375, 500 },
            { 250, 500, 750, 1000 },
            { 300, 600, 900, 1400 },
            { 350, 750, 1100, 1700 },
            { 450, 900, 1400, 2100 },
            { 550, 1100, 1600, 2400 },
            { 600, 1200, 1900, 2800 },
            { 800, 1600, 2400, 3600 },
            { 1000, 2000, 3000, 4500 },
            { 1100, 2200, 3400, 5100 },
            { 1250, 2500, 3800, 5700 },
            { 1400, 2800, 4300, 6400 },
            { 1600, 3200, 4800, 7200 },
            { 2000, 3900, 5900, 8800 },
            { 2100, 4200, 6300, 9500 },
            { 2400, 4900, 7300, 10900 },
            { 2800, 5700, 8500, 12700 }
        };

        List<float> _encounterMultipliers = new List<float>
        {
            { 1f },
            { 1.5f },
            { 2f },
            { 2f },
            { 2f },
            { 2f },
            { 2.5f },
            { 2.5f },
            { 2.5f },
            { 2.5f },
            { 3f },
            { 3f },
            { 3f },
            { 3f },
            { 4f }
        };

        List<int> _rarityLevels = new List<int>
        {
            (int)ItemRarity.Common, // 1
            (int)ItemRarity.Common, // 2
            (int)ItemRarity.Common, // 3
            (int)ItemRarity.Common, // 4
            (int)ItemRarity.Uncommon, // 5
            (int)ItemRarity.Uncommon, // 6
            (int)ItemRarity.Uncommon, // 7
            (int)ItemRarity.Uncommon, // 8
            (int)ItemRarity.Rare, // 9
            (int)ItemRarity.Rare, // 10
            (int)ItemRarity.Rare, // 11
            (int)ItemRarity.Rare, // 12
            (int)ItemRarity.Rare, // 13
            (int)ItemRarity.Rare, // 14
            (int)ItemRarity.VeryRare, // 15
            (int)ItemRarity.VeryRare, // 16
            (int)ItemRarity.VeryRare, // 17
            (int)ItemRarity.Legendary, // 18
            (int)ItemRarity.Legendary, // 19
            (int)ItemRarity.Artifact // 20
        };

        public Guid SuggestMonster(Guid userId, Guid campaignId, int difficulty, int difficultyChange, List<Guid> excludeMonsters)
        {
            UserHasAccess(userId, campaignId);
            var monster = Guid.Empty;

            using (DBReset())
            {
                //Get CR
                var crKey = Guid.Empty;
                if (difficulty > 0)
                {
                    crKey = DB.MonsterChallengeRatings.Where(x => x.Difficulty == difficulty).Select(x => x.RatingKey).FirstOrDefault();
                }
                else
                {
                    //Get Players
                    var checkCR = 1;
                    var partyLevel = GetPartyLevel(userId, campaignId);
                    DBReset();

                    checkCR = (int)Math.Max(1, partyLevel);

                    var cr = DB.MonsterChallengeRatings.Where(x => x.Challenge == checkCR.ToString()).FirstOrDefault();
                    crKey = cr.RatingKey;
                    difficulty = cr.Difficulty;
                }

                // Get all eligible monsters, then select a random one
                var monsters = DB.Monsters.Where(x => !excludeMonsters.Contains(x.MonsterKey) && x.ChallengeRating == crKey).Select(x => x.MonsterKey).ToList();
                Random rand = new Random();
                int toSkip = rand.Next(monsters.Count);

                monster = monsters.Skip(toSkip).FirstOrDefault();
                while (monster == Guid.Empty)
                {
                    if (difficultyChange > 0) { difficulty++; }
                    else { difficulty--; }

                    if (difficulty < 1 || difficulty > 34) { break; }

                    crKey = DB.MonsterChallengeRatings.Where(x => x.Difficulty == difficulty).Select(x => x.RatingKey).FirstOrDefault();

                    monsters = DB.Monsters.Where(x => !excludeMonsters.Contains(x.MonsterKey) && x.ChallengeRating == crKey).Select(x => x.MonsterKey).ToList();
                    toSkip = rand.Next(monsters.Count);
                    monster = monsters.Skip(toSkip).FirstOrDefault();
                }
            }
            

            return monster;
        }

        public List<Guid> SearchMonster(Guid userId, Guid campaignId, SearchMonsterPostModel model)
        {
            UserHasAccess(userId, campaignId);

            var monsters = new List<Monster>();
            using (DBReset())
            {
                monsters = DB.Monsters.ToList();

                if (model.Name != null && model.Name.Trim() != "")
                {
                    monsters = monsters.Where(x => x.Name.IndexOf(model.Name, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                }

                if (model.ChallengeRating != Guid.Empty)
                {
                    monsters = monsters.Where(x => x.ChallengeRating == model.ChallengeRating).ToList();
                }

                if (model.Size != Guid.Empty)
                {
                    monsters = monsters.Where(x => x.Size == model.Size).ToList();
                }

                if (model.Type != Guid.Empty)
                {
                    monsters = monsters.Where(x => x.Type == model.Type).ToList();
                }

                if (model.Environment != Guid.Empty)
                {
                    var elligible = DB.MonsterEnvironmentLinkers.Where(x => x.EnvironmentKey == model.Environment).Select(x => x.MonsterKey).ToList();
                    monsters = monsters.Where(x => elligible.Contains(x.MonsterKey)).ToList();
                }

                if (model.Alignment != null && model.Alignment.Trim() != "")
                {
                    monsters = monsters.Where(x => x.Alignment == model.Alignment).ToList();
                }
            }

            return monsters.Select(x => x.MonsterKey).ToList();
        }

        public List<int> GetThresholds(Guid userId, Guid campaignId)
        {
            UserHasAccess(userId, campaignId);

            var playerLevels = new List<int>();
            var thresholds = new List<int>() { 0, 0, 0, 0 };

            using (DBReset())
            {
                var playerKeys = DB.CampaignCharacterLinkers.Where(x => x.CampaignKey == campaignId && x.IsPlayer).Select(x => x.CharacterKey).ToList();
                if (playerKeys.Count > 0)
                {
                    playerLevels = DB.Characters.Where(x => playerKeys.Contains(x.CharacterKey)).Select(x => x.Level).ToList();

                    foreach (var level in playerLevels)
                    {
                        thresholds[0] += _xpThresholds[level - 1, 0];
                        thresholds[1] += _xpThresholds[level - 1, 1];
                        thresholds[2] += _xpThresholds[level - 1, 2];
                        thresholds[3] += _xpThresholds[level - 1, 3];
                    }
                }
                else
                {
                    thresholds = null;
                }
            }

            return thresholds;
        }

        public List<float> GetMultipliers()
        {
            return _encounterMultipliers;
        }

        public int GetPartyLevel(Guid userId, Guid campaignId)
        {
            UserHasAccess(userId, campaignId);

            var playerLevels = new List<int>();
            int partyLevel = 0;

            using (DBReset())
            {
                var playerKeys = DB.CampaignCharacterLinkers.Where(x => x.CampaignKey == campaignId && x.IsPlayer).Select(x => x.CharacterKey).ToList();
                if (playerKeys.Count > 0)
                {
                    playerLevels = DB.Characters.Where(x => playerKeys.Contains(x.CharacterKey)).Select(x => x.Level).ToList();
                    var avg = 0d;
                    foreach (var level in playerLevels) { avg += level; }
                    partyLevel = (int)Math.Round(avg / playerLevels.Count);
                }
            }

            return partyLevel;
        }

        public Guid SuggestItem(Guid userId, Guid campaignId, int rarity, int rarityChange, List<Guid> excludeItems)
        {
            UserHasAccess(userId, campaignId);

            var item = Guid.Empty;
            using (DBReset())
            {
                //Get rarity
                var rarityKey = Guid.Empty;
                if (rarity > 0)
                {
                    rarityKey = DB.MagicItemRarities.Where(x => x.Rarity == rarity).Select(x => x.RarityKey).FirstOrDefault();
                }
                else
                {
                    //Get Players
                    var checkRarity = 1;
                    var partyLevel = GetPartyLevel(userId, campaignId);
                    DBReset();

                    partyLevel = (int)Math.Max(1, partyLevel);
                    checkRarity = _rarityLevels[partyLevel - 1];

                    var _rarity = DB.MagicItemRarities.Where(x => x.Rarity == checkRarity).FirstOrDefault();
                    rarityKey = _rarity.RarityKey;
                    rarity = _rarity.Rarity;
                }

                // Get all eligible items, then select a random one
                var items = DB.MagicItems.Where(x => !excludeItems.Contains(x.ItemKey) && x.Rarity == rarityKey).Select(x => x.ItemKey).ToList();
                Random rand = new Random();
                int toSkip = rand.Next(items.Count);

                item = items.Skip(toSkip).FirstOrDefault();
                while (item == Guid.Empty)
                {
                    if (rarityChange > 0) { rarity++; }
                    else { rarity--; }

                    if (rarity < 1 || rarity > 6) { break; }

                    rarityKey = DB.MagicItemRarities.Where(x => x.Rarity == rarity).Select(x => x.RarityKey).FirstOrDefault();

                    items = DB.MagicItems.Where(x => !excludeItems.Contains(x.ItemKey) && x.Rarity == rarityKey).Select(x => x.ItemKey).ToList();
                    toSkip = rand.Next(items.Count);
                    item = items.Skip(toSkip).FirstOrDefault();
                }
            }

            return item;
        }

        public List<Guid> SearchItem(Guid userId, Guid campaignId, SearchItemPostModel model)
        {
            UserHasAccess(userId, campaignId);

            var items = new List<MagicItem>();
            using (DBReset())
            {
                items = DB.MagicItems.ToList();

                if (model.Name != null && model.Name.Trim() != "")
                {
                    items = items.Where(x => x.Name.IndexOf(model.Name, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                }

                if (model.Type != Guid.Empty)
                {
                    items = items.Where(x => x.Type == model.Type).ToList();
                }

                if (model.Rarity != Guid.Empty)
                {
                    items = items.Where(x => x.Rarity == model.Rarity).ToList();
                }

                if (model.HasAttunement != null)
                {
                    items = items.Where(x => x.HasAttunement == model.HasAttunement).ToList();
                }
            }

            return items.Select(x => x.ItemKey).ToList();
        }

        public TreasureViewModel GenerateTreasure(List<string> challengeRatings)
        {
            var treasureModel = new TreasureViewModel();
            var totalTreasure = new TreasureStorage();

            using (DBReset())
            {
                foreach (var cr in challengeRatings)
                {
                    var numCR = ConvertChallengeRating(cr);
                    var treasures = DB.Treasures.Where(x => x.MinChallenge <= numCR && x.MaxChallenge >= numCR).OrderBy(x => x.Probability).ToList();
                    var rand = new Random();
                    var dieRoll = rand.Next(100) + 1;

                    Treasure treasure = null;
                    foreach (var item in treasures)
                    {
                        if (dieRoll <= item.Probability)
                        {
                            treasure = item;
                            break;
                        }
                    }
                    if (treasure == null) { treasure = treasures.FirstOrDefault(); }

                    totalTreasure.CP += CalculateTreasure(treasure.CP);
                    totalTreasure.SP += CalculateTreasure(treasure.SP);
                    totalTreasure.EP += CalculateTreasure(treasure.EP);
                    totalTreasure.GP += CalculateTreasure(treasure.GP);
                    totalTreasure.PP += CalculateTreasure(treasure.PP);
                }

                treasureModel.Treasure = new List<string>();
                if (totalTreasure.CP > 0) { treasureModel.Treasure.Add(ConvertTreasure(totalTreasure.CP, CoinType.Copper)); }
                if (totalTreasure.SP > 0) { treasureModel.Treasure.Add(ConvertTreasure(totalTreasure.SP, CoinType.Silver)); }
                if (totalTreasure.EP > 0) { treasureModel.Treasure.Add(ConvertTreasure(totalTreasure.EP, CoinType.Electrum)); }
                if (totalTreasure.GP > 0) { treasureModel.Treasure.Add(ConvertTreasure(totalTreasure.GP, CoinType.Gold)); }
                if (totalTreasure.PP > 0) { treasureModel.Treasure.Add(ConvertTreasure(totalTreasure.PP, CoinType.Platinum)); }
            }

            return treasureModel;
        }

        public TreasureHoardViewModel GenerateTreasureHoard(string challengeRating)
        {
            var hoardModel = new TreasureHoardViewModel();

            using (DBReset())
            {
                var numCR = ConvertChallengeRating(challengeRating);
                var hoards = DB.TreasureHoards.Where(x => x.MinChallenge <= numCR && x.MaxChallenge >= numCR).OrderBy(x => x.Probability).ToList();
                var rand = new Random();
                var dieRoll = rand.Next(100) + 1;

                TreasureHoard hoard = null;
                foreach (var item in hoards)
                {
                    if (dieRoll <= item.Probability)
                    {
                        hoard = item;
                        break;
                    }
                }
                if (hoard == null) { hoard = hoards.FirstOrDefault(); }

                hoardModel.Treasure = new List<string>();
                if (hoard.CP != null) { hoardModel.Treasure.Add(ConvertTreasure(CalculateTreasure(hoard.CP), CoinType.Copper)); }
                if (hoard.SP != null) { hoardModel.Treasure.Add(ConvertTreasure(CalculateTreasure(hoard.SP), CoinType.Silver)); }
                if (hoard.EP != null) { hoardModel.Treasure.Add(ConvertTreasure(CalculateTreasure(hoard.EP), CoinType.Electrum)); }
                if (hoard.GP != null) { hoardModel.Treasure.Add(ConvertTreasure(CalculateTreasure(hoard.GP), CoinType.Gold)); }
                if (hoard.PP != null) { hoardModel.Treasure.Add(ConvertTreasure(CalculateTreasure(hoard.PP), CoinType.Platinum)); }

                if (hoard.Gemstones != null)
                {
                    var total = CalculateTreasure(hoard.Gemstones);
                    var gemstones = DB.Gemstones.Where(x => x.Value == hoard.Value).ToList();
                    hoardModel.Items = new List<TreasureItemViewModel>();
                    var runningTotal = new Dictionary<Guid, int>();
                    for (var i = 0; i < total; i++)
                    {
                        var gemstone = gemstones[rand.Next(gemstones.Count)];
                        if (!runningTotal.ContainsKey(gemstone.GemstoneKey)) { runningTotal.Add(gemstone.GemstoneKey, 0); }
                        runningTotal[gemstone.GemstoneKey] += 1;
                    }

                    foreach (var entry in runningTotal)
                    {
                        var gemstone = gemstones.FirstOrDefault(x => x.GemstoneKey == entry.Key);
                        hoardModel.Items.Add(new TreasureItemViewModel
                        {
                            Display = $"{entry.Value} {gemstone.Name}",
                            Info = $"{gemstone.Value} GP - {gemstone.Description}"
                        });
                    }
                }
                else if (hoard.ArtObjects != null)
                {
                    var total = CalculateTreasure(hoard.ArtObjects);
                    var artObjects = DB.ArtObjects.Where(x => x.Value == hoard.Value).ToList();
                    hoardModel.Items = new List<TreasureItemViewModel>();
                    var runningTotal = new Dictionary<Guid, int>();
                    for (var i = 0; i < total; i++)
                    {
                        var artObject = artObjects[rand.Next(artObjects.Count)];
                        if (!runningTotal.ContainsKey(artObject.ArtObjectKey)) { runningTotal.Add(artObject.ArtObjectKey, 0); }
                        runningTotal[artObject.ArtObjectKey] += 1;
                    }

                    foreach (var entry in runningTotal)
                    {
                        var artObject = artObjects.FirstOrDefault(x => x.ArtObjectKey == entry.Key);
                        hoardModel.Items.Add(new TreasureItemViewModel
                        {
                            Display = $"{entry.Value} {artObject.Name}",
                            Info = $"{artObject.Value} GP"
                        });
                    }
                }
            }

            return hoardModel;
        }

        public EncounterViewModel GetEncounter(Guid encounterId)
        {
            var encounter = CreateEncounter(encounterId);
            if (encounter == null)
            {
                DBReset();
                DB.EncounterMonsters.RemoveRange(x => x.EncounterKey == encounterId && (DB.Monsters.FirstOrDefault(y => y.MonsterKey == x.MonsterKey) == null));
                DB.EncounterItems.RemoveRange(x => x.EncounterKey == encounterId && (DB.MagicItems.FirstOrDefault(y => y.ItemKey == x.ItemKey) == null));

                encounter = DB.Encounters.Where(x => x.EncounterKey == encounterId).Select(x => new EncounterViewModel
                {
                    Description = x.Description,
                    EncounterKey = x.EncounterKey,
                    Items = DB.EncounterItems.Where(y => y.EncounterKey == encounterId).Select(y => new MagicItemViewModel { ItemKey = y.ItemKey}).ToList(),
                    Monsters = DB.EncounterMonsters.Where(y => y.EncounterKey == encounterId).Select(y => new EncounterMonsterViewModel { MonsterKey = y.MonsterKey, Count = y.Count }).ToList(),
                    Name = x.Name,
                    RewardSummary = x.RewardSummary
                }).FirstOrDefault();

                DB.SaveChanges();
            }

            return encounter;
        }

        public EncounterViewModel CreateEncounter(Guid encounterId)
        {
            var exists = false;
            using (DBReset())
            {
                if (DB.Encounters.FirstOrDefault(x => x.EncounterKey == encounterId) != null)
                {
                    exists = true;
                }
            }

            if (exists) { return null; }

            return new EncounterViewModel
            {
                EncounterKey = encounterId,
                Items = new List<MagicItemViewModel>(),
                Monsters = new List<EncounterMonsterViewModel>()
            };
        }

        public void UpdateEncounter(EncounterPostModel model)
        {
            using (DBReset())
            {
                var add = false;
                var encounter = DB.Encounters.FirstOrDefault(x => x.EncounterKey == model.EncounterKey);
                if (encounter == null)
                {
                    add = true;
                    encounter = new Encounter
                    {
                        EncounterKey = model.EncounterKey
                    };
                }

                encounter.Name = model.Name;
                encounter.Description = model.Description;
                encounter.RewardSummary = model.RewardSummary;

                //Monsters
                DB.EncounterMonsters.RemoveRange(x => x.EncounterKey == model.EncounterKey);
                foreach (var monster in model.Monsters)
                {
                    DB.EncounterMonsters.Add(new EncounterMonster
                    {
                        EncounterKey = model.EncounterKey,
                        MonsterKey = monster.MonsterKey,
                        Count = monster.Count
                    });
                }

                //Items
                DB.EncounterItems.RemoveRange(x => x.EncounterKey == model.EncounterKey);
                foreach (var item in model.Items)
                {
                    DB.EncounterItems.Add(new EncounterItem
                    {
                        EncounterKey = model.EncounterKey,
                        ItemKey = item.ItemKey
                    });
                }

                if (add)
                {
                    DB.Encounters.Add(encounter);
                }

                DB.SaveChanges();
            }
        }

        public void DeleteEncounter(Guid encounterId)
        {
            using (DBReset())
            {
                DB.Encounters.RemoveRange(x => x.EncounterKey == encounterId);
                DB.EncounterItems.RemoveRange(x => x.EncounterKey == encounterId);
                DB.EncounterMonsters.RemoveRange(x => x.EncounterKey == encounterId);

                DB.SaveChanges();
            }
        }

        public void DeleteDungeonEncounters(Guid dungeonKey, List<Guid> encounterKeys)
        {
            DBReset();
            var tileIds = DB.DungeonTiles.Where(x => x.DungeonKey == dungeonKey).Select(x => x.TileKey).ToList();
            var usedEncounterKeys = DB.DungeonTileEncounters.Where(x => tileIds.Contains(x.TileKey)).Select(x => x.EncounterKey).ToList();

            foreach (var key in usedEncounterKeys)
            {
                if (!encounterKeys.Contains(key))
                {
                    DeleteEncounter(key);
                }
            }
        }

        public void DeleteQuestEventEncounters(Guid arcKey, List<Guid> encounterKeys)
        {
            DBReset();
            var questIds = DB.Quests.Where(x => x.ArcKey == arcKey).Select(x => x.QuestKey).ToList();
            var eventIds = DB.QuestEvents.Where(x => questIds.Contains(x.QuestKey)).Select(x => x.EventKey).ToList();
            var usedEncounterKeys = DB.QuestEventEncounters.Where(x => eventIds.Contains(x.EventKey)).Select(x => x.EncounterKey).ToList();
            
            foreach (var key in usedEncounterKeys)
            {
                if (!encounterKeys.Contains(key))
                {
                    DeleteEncounter(key);
                }
            }
        }




        float ConvertChallengeRating(string cr)
        {
            var val = 0f;
            var nums = cr.Split('/');
            if (nums.Count() > 1)
            {
                var num1 = Int32.Parse(nums[0]);
                var num2 = Int32.Parse(nums[1]);
                val = num1 / num2;
            }
            else
            {
                val = float.Parse(nums[0]);
            }

            return val;
        }

        int CalculateTreasure(string treasure)
        {
            if (treasure == null || treasure == "") { return 0; }

            var list = treasure.Split('x');
            var dice = list[0];
            var mult = list.Count() > 1 ? Int32.Parse(list[1]) : 1;

            list = dice.Split('d');
            var numDice = Int32.Parse(list[0]);
            var die = list.Count() > 1 ? Int32.Parse(list[1]) : 6;

            var total = 0;
            for (var i = 0; i < numDice; i++)
            {
                var rand = new Random();
                total += rand.Next(die) + 1;
            }

            total *= mult;

            return total;
        }

        string ConvertTreasure(int treasure, CoinType type)
        {
            var result = treasure.ToString() + " ";
            switch (type)
            {
                case CoinType.Copper:
                    result += "CP";
                    break;
                case CoinType.Silver:
                    result += "SP";
                    break;
                case CoinType.Electrum:
                    result += "EP";
                    break;
                case CoinType.Gold:
                    result += "GP";
                    break;
                case CoinType.Platinum:
                    result += "PP";
                    break;
            }

            return result;
        }
    }

    class TreasureStorage
    {
        public int CP { get; set; } = 0;
        public int SP { get; set; } = 0;
        public int EP { get; set; } = 0;
        public int GP { get; set; } = 0;
        public int PP { get; set; } = 0;
    }
}
