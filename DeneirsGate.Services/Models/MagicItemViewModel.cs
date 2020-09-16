using System;
using System.ComponentModel.DataAnnotations;

namespace DeneirsGate.Services
{
    public class MagicItemViewModel
    {
        public Guid ItemKey { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Rarity { get; set; }
        public bool HasAttunement { get; set; }
        public string Description { get; set; }
        public int RarityValue { get; set; }
    }

    public class MagicItemEditModel
    {
        public Guid ItemKey { get; set; }
        public Guid CampaignKey { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
        [Required, NotEmptyGuid]
        public Guid Type { get; set; }
        [Required, NotEmptyGuid]
        public Guid Rarity { get; set; }
        [Display(Name = "Requires Attunement")]
        public bool HasAttunement { get; set; }
        public string Description { get; set; }
    }

    public class MagicItemPostModel : MagicItemEditModel
    {
        public bool IsCustom { get; set; }
    }

    public class MagicItemRarityViewModel
    {
        public Guid RarityKey { get; set; }
        public string Name { get; set; }
    }

    public class MagicItemTypeViewModel
    {
        public Guid TypeKey { get; set; }
        public string Name { get; set; }
    }
}
