using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeneirsGate.Services
{
    public class DungeonViewModel
    {
        public Guid DungeonKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<DungeonTileViewModel> Tiles { get; set; }
    }

    public class DungeonListViewModel
    {
        public Guid DungeonKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
    }

    public class DungeonTileViewModel
    {
        public Guid TileKey { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int? Index { get; set; }
        public DungeonTileTrapViewModel Trap { get; set; }
        public EncounterViewModel Encounter { get; set; }
    }

    public class DungeonTileTrapViewModel
    {
        public Guid TrapKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid NatureKey { get; set; }
        public Guid TypeKey { get; set; }
        public int? SaveDC { get; set; }
        public int? AttackBonus { get; set; }
        public string Damage { get; set; }
    }

    public class DungeonPostModel
    {
        [Required]
        public Guid DungeonKey { get; set; }
        [Required, StringLength(150)]
        public string Name { get; set; }
        public string Description { get; set; }
        public List<DungeonTilePostModel> Tiles { get; set; } = new List<DungeonTilePostModel>();
    }

    public class DungeonTilePostModel
    {
        public Guid TileKey { get; set; }
        [Required]
        public int Row { get; set; }
        [Required]
        public int Column { get; set; }
        public string Description { get; set; }
        [Required, StringLength(150)]
        public string Image { get; set; }
        public int? Index { get; set; }
        public DungeonTileTrapPostModel Trap { get; set; }
        public EncounterPostModel Encounter { get; set; }
    }

    public class DungeonTileTrapPostModel
    {
        [Required, StringLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public Guid NatureKey { get; set; }
        [Required]
        public Guid TypeKey { get; set; }
        public int? SaveDC { get; set; }
        public int? AttackBonus { get; set; }
        public string Damage { get; set; }
    }

    public class TrapViewModel
    {
        public Guid TrapKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Nature { get; set; }
        public string Type { get; set; }
        public int SaveDC { get; set; }
        public int AttackBonus { get; set; }
        public string Damage { get; set; }
        public Guid NatureKey { get; set; }
        public Guid TypeKey { get; set; }
    }

    public class TrapEditModel
    {
        public Guid TrapKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Display(Name = "Nature")]
        public Guid NatureKey { get; set; }
        [Display(Name = "Type")]
        public Guid TypeKey { get; set; }
    }

    public class TrapSuggestPostModel
    {
        public string Name { get; set; }
        public Guid? NatureKey { get; set; }
        public Guid? TypeKey { get; set; }
    }

    public class TrapStatsPostModel
    {
        public Guid TypeKey { get; set; }
    }

    public class TrapStatsModel
    {
        public int SaveDC { get; set; }
        public int AttackBonus { get; set; }
        public string Damage { get; set; }
    }

    public class TrapNatureViewModel
    {
        public Guid NatureKey { get; set; }
        public string Name { get; set; }
    }

    public class TrapTypeViewModel
    {
        public Guid TypeKey { get; set; }
        public string Name { get; set; }
    }

    public class TrapPostModel
    {
        [NotEmptyGuid]
        public Guid TrapKey { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        [NotEmptyGuid]
        public Guid NatureKey { get; set; }
        [NotEmptyGuid]
        public Guid TypeKey { get; set; }
    }
}
