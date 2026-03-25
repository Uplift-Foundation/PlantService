using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantService.Models;

/// <summary>
/// A plant owned by a user. Tracks growth stage and care status.
/// </summary>
public class UserPlant
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public int PlantCatalogId { get; set; }

    /// <summary>
    /// Growth stage from 1 (Seedling) to 5 (Overgrown).
    /// </summary>
    [Range(1, 5)]
    public int GrowthStage { get; set; } = 1;

    /// <summary>
    /// Water level from 0-100. Decreases over time.
    /// </summary>
    [Range(0, 100)]
    public int WaterLevel { get; set; } = 100;

    /// <summary>
    /// Sunlight level from 0-100. Decreases over time.
    /// </summary>
    [Range(0, 100)]
    public int SunlightLevel { get; set; } = 100;

    /// <summary>
    /// Nutrient level from 0-100. Decreases over time.
    /// </summary>
    [Range(0, 100)]
    public int NutrientLevel { get; set; } = 100;

    /// <summary>
    /// Accumulated growth points towards next stage.
    /// When this reaches the threshold, plant advances to next stage.
    /// </summary>
    public int GrowthProgress { get; set; } = 0;

    /// <summary>
    /// Whether the plant is placed in the greenhouse (visible in 2.5D view).
    /// </summary>
    public bool InGreenhouse { get; set; } = true;

    public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastCaredAt { get; set; }

    public DateTime? LastDecayAt { get; set; }

    // Navigation property
    [ForeignKey("PlantCatalogId")]
    public virtual PlantCatalog? PlantCatalog { get; set; }
}
