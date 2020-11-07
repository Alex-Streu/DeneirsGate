using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("CampaignDungeonLinkers")]
    public class CampaignDungeonLinker
    {
        [Key, Column(Order = 1)]
        public Guid CampaignKey { get; set; }
        [Key, Column(Order = 2)]
        public Guid DungeonKey { get; set; }
    }
}
