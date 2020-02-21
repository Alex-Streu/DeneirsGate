using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("CampaignPlayerLinkers")]
    public class CampaignPlayerLinker
    {
        [Key, Column(Order = 0)]
        public Guid CampaignKey { get; set; }

        [Key, Column(Order = 1)]
        public Guid PlayerKey { get; set; }

        [Key, Column(Order = 2)]
        public Guid CharacterKey { get; set; }
    }
}
