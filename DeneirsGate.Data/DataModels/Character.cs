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
        
        public int Proficiency { get; set; }

        public int MaxHP { get; set; }

        public int Strength { get; set; }

        public int Dexterity { get; set; }

        public int Constitution { get; set; }

        public int Intelligence { get; set; }

        public int Wisdom { get; set; }

        public int Charisma { get; set; }

        public string Abilities { get; set; }

        public string Status { get; set; }

        [StringLength(50)]
        public string Armor { get; set; }

        public int ArmorClass { get; set; }

        [StringLength(10)]
        public string SpellcastingAbility { get; set; }

        public int SpellcastingMod { get; set; }

        public int SpellSaveDC { get; set; }

        public int SpellsPerDay { get; set; }

        public int Cantrips { get; set; }

        public int Level1Spells { get; set; }

        public int Level2Spells { get; set; }

        public int Level3Spells { get; set; }

        public int Level4Spells { get; set; }

        public int Level5Spells { get; set; }

        public int Level6Spells { get; set; }

        public int Level7Spells { get; set; }

        public int Level8Spells { get; set; }

        public int Level9Spells { get; set; }

        public int Copper { get; set; }

        public int Silver { get; set; }

        public int Electrum { get; set; }

        public int Gold { get; set; }

        public int Platinum { get; set; }

        public string Inventory { get; set; }
    }
}
