namespace DeneirsGate.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Class
    {
        [Key]
        public Guid ClassKey { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
    }
}
