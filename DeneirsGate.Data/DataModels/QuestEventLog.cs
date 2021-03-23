using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("QuestEventLogs")]
    public class QuestEventLog
    {
        [Key, Column(Order = 1)]
        public Guid CampaignKey { get; set; }
        [Key, Column(Order = 2)]
        public Guid LogKey { get; set; }
        [Key, Column(Order = 3)]
        public Guid EventKey { get; set; }
    }
}
