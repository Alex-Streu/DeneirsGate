using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("Campaigns")]
    public class Campaign
    {
        [Key]
        public Guid CampaignKey { get; set; }
        
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [StringLength(200)]
        public string Portrait { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
