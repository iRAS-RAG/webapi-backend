using IRasRag.Domain.Common;
using IRasRag.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IRasRag.Infrastructure.Data
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
            modelBuilder.ApplyConfiguration(new Configurations.UserConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.RoleConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.RefreshTokenConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.VerificationConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.UserFarmConfiguration());

            // Farm
            modelBuilder.ApplyConfiguration(new Configurations.FarmConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.FishTankConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CameraConfiguration());

            // Production
            modelBuilder.ApplyConfiguration(new Configurations.FarmingBatchConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.FeedingLogConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.SpeciesConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.GrowthStageConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.SpeciesThresholdConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.MortalityLogConfiguration());

            // Feeding Configuration
            modelBuilder.ApplyConfiguration(new Configurations.FeedTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.SpeciesStageConfigConfiguration());

            // Hardware
            modelBuilder.ApplyConfiguration(new Configurations.MasterBoardConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.SensorConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.SensorTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.SensorLogConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.ControlDeviceTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.ControlDeviceConfiguration());

            // Automated Job
            modelBuilder.ApplyConfiguration(new Configurations.JobConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.JobTypeConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.JobControlMappingConfiguration());

            // Advisory & Alerts
            modelBuilder.ApplyConfiguration(new Configurations.AlertConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CorrectiveActionConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.DocumentConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.RecommendationConfiguration());

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
                        ? (v.Value.Kind == DateTimeKind.Utc ? v.Value : v.Value.ToUniversalTime())
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
                    entry.State = EntityState.Modified;
                    entry.Entity.ModifiedAt = now;
                    entry.Entity.DeletedAt = now;
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
                    entry.State = EntityState.Modified;
                    entry.Entity.ModifiedAt = now;
                    entry.Entity.DeletedAt = now;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}