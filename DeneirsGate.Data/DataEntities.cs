namespace DeneirsGate.Data
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public partial class DataEntities : DbContext
    {
        public DataEntities()
            : base("name=DataEntities")
        {
        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<C__RefactorLog> C__RefactorLog { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

        #region Campaigns
        
        public virtual DbSet<Campaign> Campaigns { get; set; }
        public virtual DbSet<UserCampaign> UserCampaigns { get; set; }
        public virtual DbSet<Arc> Arcs { get; set; }
        public virtual DbSet<ArcCharacterLinker> ArcCharacterLinkers { get; set; }
        public virtual DbSet<ArcMapPin> ArcMapPins { get; set; }
        public virtual DbSet<Quest> Quests { get; set; }
        public virtual DbSet<QuestEvent> QuestEvents { get; set; }
        public virtual DbSet<QuestEventEncounter> QuestEventEncounters { get; set; }
        public virtual DbSet<ArcTodoItem> ArcTodoItems { get; set; }
        public virtual DbSet<ActivityLog> ActivityLogs { get; set; }
        public virtual DbSet<CharacterLog> CharacterLogs { get; set; }
        public virtual DbSet<QuestEventLog> QuestEventLogs { get; set; }
        public virtual DbSet<DungeonLog> DungeonLogs { get; set; }

        #endregion

        #region Characters

        public virtual DbSet<CampaignCharacterLinker> CampaignCharacterLinkers { get; set; }
        public virtual DbSet<Background> Backgrounds { get; set; }
        public virtual DbSet<Character> Characters { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<Race> Races { get; set; }
        public virtual DbSet<CharacterWeapon> CharacterWeapons { get; set; }
        public virtual DbSet<CharacterSpell> CharacterSpells { get; set; }
        public virtual DbSet<DamageType> DamageTypes { get; set; }

        public virtual DbSet<RelationshipTree> RelationshipTrees { get; set; }
        public virtual DbSet<RelationshipTreeTier> RelationshipTreeTiers { get; set; }
        public virtual DbSet<RelationshipTreeCharacter> RelationshipTreeCharacters { get; set; }

        #endregion

        #region Monsters

        public virtual DbSet<Monster> Monsters { get; set; }
        public virtual DbSet<Environment> Environments { get; set; }
        public virtual DbSet<MonsterSize> MonsterSizes { get; set; }
        public virtual DbSet<MonsterType> MonsterTypes { get; set; }
        public virtual DbSet<MonsterChallengeRating> MonsterChallengeRatings { get; set; }
        public virtual DbSet<MonsterEnvironmentLinker> MonsterEnvironmentLinkers { get; set; }
        public virtual DbSet<UserMonster> UserMonsters { get; set; }

        #endregion

        #region Magic Items

        public virtual DbSet<MagicItem> MagicItems { get; set; }
        public virtual DbSet<MagicItemType> MagicItemTypes { get; set; }
        public virtual DbSet<MagicItemRarity> MagicItemRarities { get; set; }
        public virtual DbSet<Gemstone> Gemstones { get; set; }
        public virtual DbSet<ArtObject> ArtObjects { get; set; }
        public virtual DbSet<Treasure> Treasures { get; set; }
        public virtual DbSet<TreasureHoard> TreasureHoards { get; set; }
        public virtual DbSet<UserMagicItem> UserMagicItems { get; set; }

        #endregion

        #region Dungeons

        public virtual DbSet<Dungeon> Dungeons { get; set; }
        public virtual DbSet<DungeonTile> DungeonTiles { get; set; }
        public virtual DbSet<DungeonTileTrap> DungeonTileTraps { get; set; }
        public virtual DbSet<DungeonTileEncounter> DungeonTileEncounters { get; set; }
        public virtual DbSet<CampaignDungeonLinker> CampaignDungeonLinkers { get; set; }
        public virtual DbSet<Trap> Traps { get; set; }
        public virtual DbSet<TrapNature> TrapNatures { get; set; }
        public virtual DbSet<TrapType> TrapTypes { get; set; }
        public virtual DbSet<TrapTypeDamage> TrapTypeDamages { get; set; }
        public virtual DbSet<CampaignTrapLinker> CampaignTrapLinkers { get; set; }

        #endregion

        #region Settlements

        public virtual DbSet<Settlement> Settlements { get; set; }
        public virtual DbSet<SettlementLocation> SettlementLocations { get; set; }

        #endregion

        #region Encounters

        public virtual DbSet<Encounter> Encounters { get; set; }
        public virtual DbSet<EncounterMonster> EncounterMonsters { get; set; }
        public virtual DbSet<EncounterItem> EncounterItems { get; set; }

        #endregion

        #region Tutorials

        public virtual DbSet<Tutorial> Tutorials { get; set; }
        public virtual DbSet<UserTutorial> UserTutorials { get; set; }

        #endregion

        #region Social

        public virtual DbSet<FriendRequest> FriendRequests { get; set; }
        public virtual DbSet<FriendBlock> FriendBlocks { get; set; }

        #endregion

        #region Suggestion Box

        public virtual DbSet<Suggestion> Suggestions { get; set; }

        #endregion

        #region

        public virtual DbSet<Notification> Notifications { get; set; }

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            //modelBuilder.Entity<Role>()
            //    .Property(e => e.Name)
            //    .IsFixedLength();
        }
    }

    public static class DataExtensions
    {
        public static void RemoveRange<TEntity>(
            this DbSet<TEntity> entities,
            System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            var records = entities
                .Where(predicate)
                .ToList();
            if (records.Count > 0)
                entities.RemoveRange(records);
        }
    }
}
