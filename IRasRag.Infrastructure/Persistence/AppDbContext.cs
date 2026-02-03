using IRasRag.Domain.Common;
using IRasRag.Domain.Entities;
using IRasRag.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IRasRag.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        // ====================================
        // User and Auth
        // ====================================
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Verification> Verifications { get; set; }
        public DbSet<UserFarm> UserFarms { get; set; }

        // ====================================
        // Farm
        // ====================================
        public DbSet<Farm> Farms { get; set; }
        public DbSet<FishTank> FishTanks { get; set; }
        public DbSet<Camera> Cameras { get; set; }

        // ====================================
        // Production
        // ====================================
        public DbSet<FarmingBatch> FarmingBatches { get; set; }
        public DbSet<FeedingLog> FeedingLogs { get; set; }
        public DbSet<Species> Species { get; set; }
        public DbSet<GrowthStage> GrowthStages { get; set; }
        public DbSet<SpeciesThreshold> SpeciesThresholds { get; set; }
        public DbSet<MortalityLog> MortalityLogs { get; set; }

        // ====================================
        // Feeding Configuration
        // ====================================
        public DbSet<FeedType> FeedTypes { get; set; }
        public DbSet<SpeciesStageConfig> SpeciesStageConfigs { get; set; }

        // ====================================
        // Hardware
        // ====================================
        public DbSet<MasterBoard> MasterBoards { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<SensorType> SensorTypes { get; set; }
        public DbSet<SensorLog> SensorLogs { get; set; }
        public DbSet<ControlDeviceType> ControlDeviceTypes { get; set; }
        public DbSet<ControlDevice> ControlDevices { get; set; }

        // ====================================
        // Automated Job
        // ====================================
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobType> JobTypes { get; set; }
        public DbSet<JobControlMapping> JobControlMappings { get; set; }

        // ====================================
        // Advisory & Alerts
        // ====================================
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<CorrectiveAction> CorrectiveActions { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Recommendation> Recommendations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ====================================
            // Apply Configurations
            // ====================================

            // User and Auth
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
            modelBuilder.ApplyConfiguration(new VerificationConfiguration());
            modelBuilder.ApplyConfiguration(new UserFarmConfiguration());

            // Farm
            modelBuilder.ApplyConfiguration(new FarmConfiguration());
            modelBuilder.ApplyConfiguration(new FishTankConfiguration());
            modelBuilder.ApplyConfiguration(new CameraConfiguration());

            // Production
            modelBuilder.ApplyConfiguration(new FarmingBatchConfiguration());
            modelBuilder.ApplyConfiguration(new FeedingLogConfiguration());
            modelBuilder.ApplyConfiguration(new SpeciesConfiguration());
            modelBuilder.ApplyConfiguration(new GrowthStageConfiguration());
            modelBuilder.ApplyConfiguration(new SpeciesThresholdConfiguration());
            modelBuilder.ApplyConfiguration(new MortalityLogConfiguration());

            // Feeding Configuration
            modelBuilder.ApplyConfiguration(new FeedTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SpeciesStageConfigConfiguration());

            // Hardware
            modelBuilder.ApplyConfiguration(new MasterBoardConfiguration());
            modelBuilder.ApplyConfiguration(new SensorConfiguration());
            modelBuilder.ApplyConfiguration(new SensorTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SensorLogConfiguration());
            modelBuilder.ApplyConfiguration(new ControlDeviceTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ControlDeviceConfiguration());

            // Automated Job
            modelBuilder.ApplyConfiguration(new JobConfiguration());
            modelBuilder.ApplyConfiguration(new JobTypeConfiguration());
            modelBuilder.ApplyConfiguration(new JobControlMappingConfiguration());

            // Advisory & Alerts
            modelBuilder.ApplyConfiguration(new AlertConfiguration());
            modelBuilder.ApplyConfiguration(new CorrectiveActionConfiguration());
            modelBuilder.ApplyConfiguration(new DocumentConfiguration());
            modelBuilder.ApplyConfiguration(new RecommendationConfiguration());

            // ====================================
            // Global Value Converters
            // ====================================

            // Ensures DateTime properties are treated as UTC
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v =>
                    v.HasValue
                        ? v.Value.Kind == DateTimeKind.Utc
                            ? v.Value
                            : v.Value.ToUniversalTime()
                        : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v
            );

            // Apply converters to all entities
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(nullableDateTimeConverter);
                    }
                    else if (property.ClrType.IsEnum)
                    {
                        var converterType = typeof(EnumToStringConverter<>).MakeGenericType(
                            property.ClrType
                        );
                        var converter = (ValueConverter)Activator.CreateInstance(converterType);
                        property.SetValueConverter(converter);
                    }
                }
            }
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            var now = DateTime.UtcNow;
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.ModifiedAt = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedAt = now;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    // Check if entity supports soft delete
                    if (entry.Entity is ISoftDeletable softDeletable)
                    {
                        // Soft delete
                        entry.State = EntityState.Modified;
                        softDeletable.IsDeleted = true;
                        softDeletable.DeletedAt = now;
                        entry.Entity.ModifiedAt = now;
                    }
                }
            }
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default
        )
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            var now = DateTime.UtcNow;
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.ModifiedAt = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedAt = now;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    // Check if entity supports soft delete
                    if (entry.Entity is ISoftDeletable softDeletable)
                    {
                        // Soft delete
                        entry.State = EntityState.Modified;
                        softDeletable.IsDeleted = true;
                        softDeletable.DeletedAt = now;
                        entry.Entity.ModifiedAt = now;
                    }
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
