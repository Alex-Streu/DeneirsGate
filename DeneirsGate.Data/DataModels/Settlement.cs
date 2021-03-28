using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("Settlements")]
    public class Settlement
    {
        [Key]
        public Guid SettlementKey { get; set; }
        public Guid CampaignKey { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Map { get; set; }
        public virtual ICollection<SettlementLocation> SettlementLocations { get; set; }
    }
}
