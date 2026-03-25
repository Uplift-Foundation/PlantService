using System.ComponentModel.DataAnnotations;

namespace PlantService.Models;

/// <summary>
/// Master catalog of all available plants in the game.
/// This is seed data that defines what plants can be obtained.
/// </summary>
public class PlantCatalog
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? LatinName { get; set; }

    [MaxLength(10)]
    public string Emoji { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public PlantRarity Rarity { get; set; } = PlantRarity.Common;

    public LightPreference LightPreference { get; set; } = LightPreference.Indirect;

    public WaterNeeds WaterNeeds { get; set; } = WaterNeeds.Medium;

    /// <summary>
    /// Weight for gacha drop rates. Higher = more likely to drop.
    /// Common plants should have higher weights than legendary.
    /// </summary>
    public int GachaWeight { get; set; } = 100;

    /// <summary>
    /// Whether this plant is currently available in the gacha pool.
    /// </summary>
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual ICollection<UserPlant>? UserPlants { get; set; }
}
