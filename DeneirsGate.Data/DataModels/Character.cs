namespace DeneirsGate.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Characters")]
    public partial class Character
    {
        [Key]
        public Guid CharacterKey { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(250)]
        public string Portrait { get; set; }

        public Guid RaceKey { get; set; }

        public Guid ClassKey { get; set; }

        public Guid BackgroundKey { get; set; }

        [StringLength(250)]
        public string Fears { get; set; }

        [StringLength(250)]
        public string Ideals { get; set; }

        public string Backstory { get; set; }

        [StringLength(250)]
        public string Languages { get; set; }

        [Required, StringLength(2)]
        public string Alignment { get; set; }

        public int Level { get; set; }

        public int MaxHP { get; set; }

        public int Strength { get; set; }

        public int Dexterity { get; set; }

        public int Constitution { get; set; }

        public int Intelligence { get; set; }

        public int Wisdom { get; set; }

        public int Charisma { get; set; }

        public string Abilities { get; set; }

        public string Status { get; set; }
    }
}
