using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{

    [Table("MonsterSizes")]
    public class MonsterSize
    {
        [Key]
        public Guid SizeKey { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public int SortOrder { get; set; }
    }
}
