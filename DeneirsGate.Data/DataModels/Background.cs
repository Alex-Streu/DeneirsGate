namespace DeneirsGate.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Background
    {
        [Key]
        public Guid BackgroundKey { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
