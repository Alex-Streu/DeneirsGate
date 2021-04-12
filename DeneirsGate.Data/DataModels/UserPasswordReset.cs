using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("UserPasswordResets")]
    public class UserPasswordReset
    {
        [Key,  StringLength(256)]
        public string Code { get; set; }
        public Guid UserKey { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
