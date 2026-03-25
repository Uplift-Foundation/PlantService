using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PlantService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlantCatalogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LatinName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Emoji = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Rarity = table.Column<string>(type: "text", nullable: false),
                    LightPreference = table.Column<string>(type: "text", nullable: false),
                    WaterNeeds = table.Column<string>(type: "text", nullable: false),
                    GachaWeight = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantCatalogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPlants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PlantCatalogId = table.Column<int>(type: "integer", nullable: false),
                    GrowthStage = table.Column<int>(type: "integer", nullable: false),
                    WaterLevel = table.Column<int>(type: "integer", nullable: false),
                    SunlightLevel = table.Column<int>(type: "integer", nullable: false),
                    NutrientLevel = table.Column<int>(type: "integer", nullable: false),
                    GrowthProgress = table.Column<int>(type: "integer", nullable: false),
                    InGreenhouse = table.Column<bool>(type: "boolean", nullable: false),
                    AcquiredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastCaredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastDecayAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPlants_PlantCatalogs_PlantCatalogId",
                        column: x => x.PlantCatalogId,
                        principalTable: "PlantCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "PlantCatalogs",
                columns: new[] { "Id", "CreatedAt", "Description", "Emoji", "GachaWeight", "IsActive", "LatinName", "LightPreference", "Name", "Rarity", "WaterNeeds" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A hardy trailing vine that thrives on neglect. Perfect for beginners.", "🌿", 100, true, "Epipremnum aureum", "Low", "Pothos", "Common", "Low" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Produces cute baby plants on long stems. Great air purifier.", "🌱", 100, true, "Chlorophytum comosum", "Indirect", "Spider Plant", "Common", "Medium" },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sword-like leaves that practically thrive on neglect.", "🪴", 100, true, "Sansevieria trifasciata", "Low", "Snake Plant", "Common", "Low" },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A classic cactus with paddle-shaped segments.", "🌵", 100, true, "Opuntia", "Direct", "Prickly Pear", "Common", "Low" },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lush, feathery fronds that love humidity.", "🌿", 50, true, "Nephrolepis exaltata", "Indirect", "Fern", "Uncommon", "High" },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Elegant white blooms that brighten any room.", "🌼", 50, true, "Spathiphyllum", "Low", "Peace Lily", "Uncommon", "Medium" },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A whimsical fungi friend for your collection.", "🍄", 50, true, "Amanita muscaria", "Low", "Red Mushroom", "Uncommon", "High" },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A tropical beauty with dramatic split leaves. Loves humidity and indirect light.", "🌿", 25, true, "Monstera deliciosa", "Indirect", "Monstera", "Rare", "Medium" },
                    { 9, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Large violin-shaped leaves make this a statement piece.", "🌳", 25, true, "Ficus lyrata", "Bright", "Fiddle Leaf Fig", "Rare", "Medium" },
                    { 10, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cascading strings of bead-like leaves.", "💮", 25, true, "Senecio rowleyanus", "Bright", "String of Pearls", "Rare", "Low" },
                    { 11, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Delicate pink blooms that symbolize renewal and hope.", "🌸", 10, true, "Prunus serrulata", "Bright", "Cherry Blossom", "Epic", "Medium" },
                    { 12, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Exotic blooms that look like tropical birds in flight.", "🌺", 10, true, "Strelitzia reginae", "Direct", "Bird of Paradise", "Epic", "Medium" },
                    { 13, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A magical tree with bark that peels to reveal rainbow colors.", "🌈", 3, true, "Eucalyptus deglupta", "Direct", "Rainbow Eucalyptus", "Legendary", "High" },
                    { 14, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "The rarest bloom in the world. Smells terrible but looks amazing!", "🥀", 2, true, "Amorphophallus titanum", "Indirect", "Corpse Flower", "Legendary", "High" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlantCatalogs_IsActive",
                table: "PlantCatalogs",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PlantCatalogs_Rarity",
                table: "PlantCatalogs",
                column: "Rarity");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlants_PlantCatalogId",
                table: "UserPlants",
                column: "PlantCatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlants_UserId",
                table: "UserPlants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlants_UserId_PlantCatalogId",
                table: "UserPlants",
                columns: new[] { "UserId", "PlantCatalogId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPlants");

            migrationBuilder.DropTable(
                name: "PlantCatalogs");
        }
    }
}
