using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("UserPlayerLinkers")]
    public class UserPlayerLinker
    {
        [Key, Column(Order = 0)]
        public Guid UserKey { get; set; }

        [Key, Column(Order = 1)]
        public Guid PlayerKey { get; set; }
    }
}
