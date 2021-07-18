using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("UserCharacterRequests")]
    public class UserCharacterRequest
    {
        [Key]
        public Guid RequestKey { get; set; }
        [Required]
        public Guid OwnerUserKey { get; set; }
        [Required]
        public Guid PlayerUserKey { get; set; }
        [Required]
        public string CharacterShortKey { get; set; }
        [Required]
        public int SentTo { get; set; }
    }
}
