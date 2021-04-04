using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("UserTutorials")]
    public class UserTutorial
    {
        [Key, Column(Order = 1)]
        public Guid UserKey { get; set; }

        [Key, Column(Order = 2)]
        public Guid TutorialKey { get; set; }

        [Required]
        public bool IsComplete { get; set; }

        [Required]
        public int LastStep { get; set; }
    }
}
