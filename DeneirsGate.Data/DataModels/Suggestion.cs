using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("Suggestions")]
    public class Suggestion
    {
        [Key]
        public Guid SuggestionKey { get; set; }
        public Guid UserKey { get; set; }
        [Required, StringLength(250)]
        public string SuggestionText { get; set; }
        [Required]
        public int Type { get; set; }
        [Required]
        public int Status { get; set; }
        [Required]
        public DateTime DateSubmitted { get; set; }
    }
}
