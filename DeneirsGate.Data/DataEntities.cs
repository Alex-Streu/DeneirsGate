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

        #endregion

        #region Social

        public virtual DbSet<FriendRequest> FriendRequests { get; set; }
        public virtual DbSet<FriendBlock> FriendBlocks { get; set; }

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
