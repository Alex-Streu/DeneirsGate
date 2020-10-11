using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeneirsGate.Services
{
    public class EncounterViewModel
    {
        public Guid EncounterKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TotalXP { get; set; }
        public List<EncounterMonsterViewModel> Monsters { get; set; }
        public string RewardSummary { get; set; }
        public List<MagicItemViewModel> Items { get; set; }
    }

    public class EncounterMonsterViewModel : MonsterViewModel
    {
        public int Count { get; set; }
    }

    public class EncounterPostModel
    {
        public Guid EncounterKey { get; set; }
        [StringLength(150), Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public List<EncounterMonsterPostModel> Monsters { get; set; } = new List<EncounterMonsterPostModel>();
        public string RewardSummary { get; set; }
        public List<EncounterItemPostModel> Items { get; set; } = new List<EncounterItemPostModel>();
    }

    public class EncounterMonsterPostModel
    {
        public Guid MonsterKey { get; set; }
        public int Count { get; set; }
    }

    public class EncounterItemPostModel
    {
        public Guid ItemKey { get; set; }
    }

    public class SearchMonsterPostModel
    {
        public string Name { get; set; }
        public Guid Size { get; set; }
        public Guid Type { get; set; }
        public Guid ChallengeRating { get; set; }
        public Guid Environment { get; set; }
        public string Alignment { get; set; }
    }

    public class SuggestMonsterPostModel
    {
        public int Difficulty { get; set; }
        public int DifficultyChange { get; set; } = 0;
        public List<Guid> ExcludeMonsters { get; set; } = new List<Guid>();
    }

    public class EncounterCalculatorsViewModel
    {
        public List<int> Thresholds { get; set; }
        public List<float> Multipliers { get; set; }
    }

    public class SearchItemPostModel
    {
        public string Name { get; set; }
        public Guid Type { get; set; }
        public Guid Rarity { get; set; }
        public bool? HasAttunement { get; set; }
    }

    public class SuggestItemPostModel
    {
        public int Rarity { get; set; }
        public int RarityChange { get; set; } = 0;
        public List<Guid> ExcludeItems { get; set; } = new List<Guid>();
    }

    public class TreasureViewModel
    {
        public List<string> Treasure { get; set; }
    }

    public class TreasureHoardViewModel
    {
        public List<string> Treasure { get; set; }
        public List<TreasureItemViewModel> Items { get; set; }
    }

    public class TreasureItemViewModel
    {
        public string Display { get; set; }
        public string Info { get; set; }
    }

    public class GenerateTreasurePostModel
    {
        public List<string> ChallengeRatings { get; set; }
    }

    public class GenerateTreasureHoardPostModel
    {
        public string ChallengeRating { get; set; }
    }
}
