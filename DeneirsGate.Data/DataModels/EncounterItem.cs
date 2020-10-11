using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("EncounterItems")]
    public class EncounterItem
    {
        [Key, Column(Order = 0)]
        public Guid EncounterKey { get; set; }
        [Key, Column(Order = 1)]
        public Guid ItemKey { get; set; }
    }
}
