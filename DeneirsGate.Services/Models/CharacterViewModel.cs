using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DeneirsGate.Services
{
    public class CharacterViewModel
    {
        public Guid CharacterKey { get; set; }
        public int Proficiency { get; set; } = 2;
        public int Level { get; set; } = 1;
        [DisplayName("Max HP")]
        public int MaxHP { get; set; } = 1;
        public int Strength { get; set; } = 10;
        public int Dexterity { get; set; } = 10;
        public int Constitution { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public int Wisdom { get; set; } = 10;
        public int Charisma { get; set; } = 10;
        public string Abilities { get; set; }
        public string Status { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [DisplayName("Character Portrait")]
        public string Portrait { get; set; }
        [DisplayName("Race")]
        public Guid RaceKey { get; set; }
        public string Race { get; set; }
        [DisplayName("Class")]
        public Guid ClassKey { get; set; }
        public string Class { get; set; }
        [DisplayName("Background")]
        public Guid BackgroundKey { get; set; }
        public string Background { get; set; }
        public string Fears { get; set; }
        public string Ideals { get; set; }
        public string Backstory { get; set; }
        public string Languages { get; set; }
        public string Alignment { get; set; }
        public string Armor { get; set; }
        public int ArmorClass { get; set; } = 10;
        [DisplayName("Spellcasting Ability")]
        public string SpellcastingAbility { get; set; } = "None";
        public int SpellcastingMod { get; set; } = 0;
        public int SpellSaveDC { get; set; } = 10;
        public int SpellsPerDay { get; set; } = 0;
        public int Cantrips { get; set; } = 0;
        public int Level1Spells { get; set; } = 0;
        public int Level2Spells { get; set; } = 0;
        public int Level3Spells { get; set; } = 0;
        public int Level4Spells { get; set; } = 0;
        public int Level5Spells { get; set; } = 0;
        public int Level6Spells { get; set; } = 0;
        public int Level7Spells { get; set; } = 0;
        public int Level8Spells { get; set; } = 0;
        public int Level9Spells { get; set; } = 0;
        public int Copper { get; set; } = 0;
        public int Silver { get; set; } = 0;
        public int Electrum { get; set; } = 0;
        public int Gold { get; set; } = 0;
        public int Platinum { get; set; } = 0;
        public string Inventory { get; set; }
        public List<CharacterWeaponViewModel> Weapons { get; set; } = new List<CharacterWeaponViewModel>();
        public List<CharacterSpellViewModel> Spells { get; set; } = new List<CharacterSpellViewModel>();
        public DateTime LastUpdateDate { get; set; }
    }

    public class CharacterShortViewModel
    {
        public Guid CharacterKey { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Level { get; set; }
        public string Race { get; set; }
        public string Class { get; set; }
        public string Portrait { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }

    public class CharacterPostModel
    {
        [NotEmptyGuid]
        public Guid CharacterKey { get; set; }
        public int Level { get; set; }
        public int MaxHP { get; set; }
        public int Proficiency { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }
        public string Abilities { get; set; }
        public string Status { get; set; }
        [Required(ErrorMessage = "First Name is required!"), StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
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
        public string Alignment { get; set; }
        [StringLength(25)]
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
        public List<CharacterWeaponViewModel> Weapons { get; set; } = new List<CharacterWeaponViewModel>();
        public List<CharacterSpellViewModel> Spells { get; set; } = new List<CharacterSpellViewModel>();
        public bool IsPlayer { get; set; }
    }

    public class PlayerViewModel : CharacterViewModel
    {
        public Guid UserKey { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
    }

    public class PlayerShortViewModel : CharacterShortViewModel
    {
        public Guid UserKey { get; set; }
    }

    public class PlayerPostModel : CharacterPostModel
    {
        public Guid UserKey { get; set; }
    }

    public class CharacterWeaponViewModel
    {
        public Guid WeaponKey { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public int AttackMod { get; set; }
        [StringLength(10)]
        public string DamageDice { get; set; }
        public int DamageMod { get; set; }
        public Guid DamageType { get; set; }
        public string DamageDisplay { get; set; }
    }

    public class CharacterSpellViewModel
    {
        public Guid SpellKey { get; set; }
        public int Level { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
    }
}
