using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeneirsGate.Services
{
    public class RelationshipTreeViewModel
    {
        public Guid TreeKey { get; set; }
        public Guid CampaignKey { get; set; }
        public string Name { get; set; }
        public List<RelationshipTreeTierViewModel> Tiers { get; set; }
        public List<RelationshipTreeCharacterViewModel> AvailableCharacters { get; set; }
    }

    public class RelationshipTreeTierViewModel
    {
        public Guid TierKey { get; set; }
        public int SortOrder { get; set; }
        public List<RelationshipTreeCharacterViewModel> Characters { get; set; }
    }

    public class RelationshipTreeCharacterViewModel
    {
        public Guid CharacterKey { get; set; }
        public string Portrait { get; set; }
        public string Name { get; set; }
        public string Backstory { get; set; }
        public bool IsShallow { get; set; }
    }

    public class RelationshipTreeCharacterGetModel
    {
        public Guid CampaignKey { get; set; }
        public Guid TreeKey { get; set; }
    }

    public class RelationshipTreePostModel
    {
        public Guid TreeKey { get; set; }
        public Guid CampaignKey { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
        public List<RelationshipTreeTierViewModel> Tiers { get; set; }
    }

    public class RelationshipTreeSearchModel
    {
        public Guid TreeKey { get; set; }
        public string Name { get; set; }
        public string CharacterList { get; set; }
    }
}
