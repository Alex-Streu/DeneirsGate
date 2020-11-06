using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("ArcMapPins")]
    public class ArcMapPin
    {
        [Key]
        public Guid PinKey { get; set; }
        [Required]
        public Guid ArcKey { get; set; }
        [Required]
        public Guid QuestKey { get; set; }
        [Required]
        public double X { get; set; }
        [Required]
        public double Y { get; set; }
    }
}
