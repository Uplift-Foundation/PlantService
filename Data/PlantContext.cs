using Microsoft.EntityFrameworkCore;
using PlantService.Models;

namespace PlantService.Data;

public class PlantContext : DbContext
{
    public PlantContext(DbContextOptions<PlantContext> options) : base(options)
    {
    }

    public DbSet<PlantCatalog> PlantCatalogs { get; set; }
    public DbSet<UserPlant> UserPlants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PlantCatalog configuration
        modelBuilder.Entity<PlantCatalog>(entity =>
        {
            entity.HasIndex(e => e.Rarity);
            entity.HasIndex(e => e.IsActive);

            // Store enums as strings for readability
            entity.Property(e => e.Rarity)
                .HasConversion<string>();
            entity.Property(e => e.LightPreference)
                .HasConversion<string>();
            entity.Property(e => e.WaterNeeds)
                .HasConversion<string>();
        });

        // UserPlant configuration
        modelBuilder.Entity<UserPlant>(entity =>
        {
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.UserId, e.PlantCatalogId });

            entity.HasOne(e => e.PlantCatalog)
                .WithMany(p => p.UserPlants)
                .HasForeignKey(e => e.PlantCatalogId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed initial plant catalog data
        SeedPlantCatalog(modelBuilder);
    }

    private void SeedPlantCatalog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlantCatalog>().HasData(
            // Common plants (high gacha weight)
            new PlantCatalog
            {
                Id = 1,
                Name = "Pothos",
                LatinName = "Epipremnum aureum",
                Emoji = "\U0001F33F", // herb
                Description = "A hardy trailing vine that thrives on neglect. Perfect for beginners.",
                Rarity = PlantRarity.Common,
                LightPreference = LightPreference.Low,
                WaterNeeds = WaterNeeds.Low,
                GachaWeight = 100,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new PlantCatalog
            {
                Id = 2,
                Name = "Spider Plant",
                LatinName = "Chlorophytum comosum",
                Emoji = "\U0001F331", // seedling
                Description = "Produces cute baby plants on long stems. Great air purifier.",
                Rarity = PlantRarity.Common,
                LightPreference = LightPreference.Indirect,
                WaterNeeds = WaterNeeds.Medium,
                GachaWeight = 100,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new PlantCatalog
            {
                Id = 3,
                Name = "Snake Plant",
                LatinName = "Sansevieria trifasciata",
                Emoji = "\U0001FAB4", // potted plant
                Description = "Sword-like leaves that practically thrive on neglect.",
                Rarity = PlantRarity.Common,
                LightPreference = LightPreference.Low,
                WaterNeeds = WaterNeeds.Low,
                GachaWeight = 100,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new PlantCatalog
            {
                Id = 4,
                Name = "Prickly Pear",
                LatinName = "Opuntia",
                Emoji = "\U0001F335", // cactus
                Description = "A classic cactus with paddle-shaped segments.",
                Rarity = PlantRarity.Common,
                LightPreference = LightPreference.Direct,
                WaterNeeds = WaterNeeds.Low,
                GachaWeight = 100,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },

            // Uncommon plants
            new PlantCatalog
            {
                Id = 5,
                Name = "Fern",
                LatinName = "Nephrolepis exaltata",
                Emoji = "\U0001F33F", // herb
                Description = "Lush, feathery fronds that love humidity.",
                Rarity = PlantRarity.Uncommon,
                LightPreference = LightPreference.Indirect,
                WaterNeeds = WaterNeeds.High,
                GachaWeight = 50,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new PlantCatalog
            {
                Id = 6,
                Name = "Peace Lily",
                LatinName = "Spathiphyllum",
                Emoji = "\U0001F33C", // blossom
                Description = "Elegant white blooms that brighten any room.",
                Rarity = PlantRarity.Uncommon,
                LightPreference = LightPreference.Low,
                WaterNeeds = WaterNeeds.Medium,
                GachaWeight = 50,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new PlantCatalog
            {
                Id = 7,
                Name = "Red Mushroom",
                LatinName = "Amanita muscaria",
                Emoji = "\U0001F344", // mushroom
                Description = "A whimsical fungi friend for your collection.",
                Rarity = PlantRarity.Uncommon,
                LightPreference = LightPreference.Low,
                WaterNeeds = WaterNeeds.High,
                GachaWeight = 50,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },

            // Rare plants
            new PlantCatalog
            {
                Id = 8,
                Name = "Monstera",
                LatinName = "Monstera deliciosa",
                Emoji = "\U0001F33F", // herb (monstera doesn't have specific emoji)
                Description = "A tropical beauty with dramatic split leaves. Loves humidity and indirect light.",
                Rarity = PlantRarity.Rare,
                LightPreference = LightPreference.Indirect,
                WaterNeeds = WaterNeeds.Medium,
                GachaWeight = 25,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new PlantCatalog
            {
                Id = 9,
                Name = "Fiddle Leaf Fig",
                LatinName = "Ficus lyrata",
                Emoji = "\U0001F333", // deciduous tree
                Description = "Large violin-shaped leaves make this a statement piece.",
                Rarity = PlantRarity.Rare,
                LightPreference = LightPreference.Bright,
                WaterNeeds = WaterNeeds.Medium,
                GachaWeight = 25,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new PlantCatalog
            {
                Id = 10,
                Name = "String of Pearls",
                LatinName = "Senecio rowleyanus",
                Emoji = "\U0001F4AE", // white flower
                Description = "Cascading strings of bead-like leaves.",
                Rarity = PlantRarity.Rare,
                LightPreference = LightPreference.Bright,
                WaterNeeds = WaterNeeds.Low,
                GachaWeight = 25,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },

            // Epic plants
            new PlantCatalog
            {
                Id = 11,
                Name = "Cherry Blossom",
                LatinName = "Prunus serrulata",
                Emoji = "\U0001F338", // cherry blossom
                Description = "Delicate pink blooms that symbolize renewal and hope.",
                Rarity = PlantRarity.Epic,
                LightPreference = LightPreference.Bright,
                WaterNeeds = WaterNeeds.Medium,
                GachaWeight = 10,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new PlantCatalog
            {
                Id = 12,
                Name = "Bird of Paradise",
                LatinName = "Strelitzia reginae",
                Emoji = "\U0001F33A", // hibiscus
                Description = "Exotic blooms that look like tropical birds in flight.",
                Rarity = PlantRarity.Epic,
                LightPreference = LightPreference.Direct,
                WaterNeeds = WaterNeeds.Medium,
                GachaWeight = 10,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },

            // Legendary plants
            new PlantCatalog
            {
                Id = 13,
                Name = "Rainbow Eucalyptus",
                LatinName = "Eucalyptus deglupta",
                Emoji = "\U0001F308", // rainbow
                Description = "A magical tree with bark that peels to reveal rainbow colors.",
                Rarity = PlantRarity.Legendary,
                LightPreference = LightPreference.Direct,
                WaterNeeds = WaterNeeds.High,
                GachaWeight = 3,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new PlantCatalog
            {
                Id = 14,
                Name = "Corpse Flower",
                LatinName = "Amorphophallus titanum",
                Emoji = "\U0001F940", // wilted flower
                Description = "The rarest bloom in the world. Smells terrible but looks amazing!",
                Rarity = PlantRarity.Legendary,
                LightPreference = LightPreference.Indirect,
                WaterNeeds = WaterNeeds.High,
                GachaWeight = 2,
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
