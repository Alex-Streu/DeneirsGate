using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("Encounters")]
    public class Encounter
    {
        [Key]
        public Guid EncounterKey { get; set; }
        [StringLength(150), Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string RewardSummary { get; set; }
    }
}
