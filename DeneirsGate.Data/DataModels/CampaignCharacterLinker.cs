using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("CampaignCharacterLinkers")]
    public class CampaignCharacterLinker
    {
        [Key, Column(Order = 0)]
        public Guid CampaignKey { get; set; }

        [Key, Column(Order = 1)]
        public Guid CharacterKey { get; set; }
    }
}
