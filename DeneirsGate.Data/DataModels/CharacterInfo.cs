namespace DeneirsGate.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CharacterInfos")]
    public partial class CharacterInfo
    {
        [Key]
        public Guid CharacterKey { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        public string PortraitUrl { get; set; }

        public Guid RaceKey { get; set; }

        public Guid ClassKey { get; set; }

        public int Level { get; set; }

        public Guid BackgroundKey { get; set; }

        public string Bio { get; set; }
    }
}
