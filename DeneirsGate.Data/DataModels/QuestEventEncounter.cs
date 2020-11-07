using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("QuestEventEncounters")]
    public class QuestEventEncounter
    {
        [Key, Column(Order = 1)]
        public Guid EventKey { get; set; }
        [Key, Column(Order = 2)]
        public Guid EncounterKey { get; set; }
    }
}
