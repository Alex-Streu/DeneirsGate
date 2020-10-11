using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("CampaignTrapLinkers")]
    public class CampaignTrapLinker
    {
        [Key, Column(Order = 0)]
        public Guid CampaignKey { get; set; }

        [Key, Column(Order = 1)]
        public Guid TrapKey { get; set; }
    }
}
