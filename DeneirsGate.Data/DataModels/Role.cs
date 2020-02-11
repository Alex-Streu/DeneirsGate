namespace DeneirsGate.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Role
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Name { get; set; }

        public int Priviledge { get; set; }
    }
}
