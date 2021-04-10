using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("ArcTodoItems")]
    public class ArcTodoItem
    {
        [Key]
        public Guid ItemKey { get; set; }
        [Required]
        public Guid ArcKey { get; set; }
        [Required]
        public DateTime DateLogged { get; set; }
        [Required, StringLength(50)]
        public string Text { get; set; }
    }
}
