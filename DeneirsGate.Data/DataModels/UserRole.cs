namespace DeneirsGate.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserRole
    {
        [Key]
        [Column(Order = 0)]
        public Guid UserFK { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid RoleFK { get; set; }
    }
}
