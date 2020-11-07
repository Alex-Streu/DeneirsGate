using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("CharacterLogs")]
    public class CharacterLog
    {
        [Key, Column(Order = 1)]
        public Guid LogKey { get; set; }
        [Key, Column(Order = 2)]
        public Guid CharacterKey { get; set; }
    }
}
