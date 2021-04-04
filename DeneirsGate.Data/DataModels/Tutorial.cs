using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("Tutorials")]
    public class Tutorial
    {
        [Key]
        public Guid TutorialKey { get; set; }

        [Required, StringLength(100)]
        public string Route { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }
    }
}
