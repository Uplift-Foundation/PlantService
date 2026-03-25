namespace PlantService.Models;

/// <summary>
/// DTO for plant catalog items (read-only reference data).
/// </summary>
public class PlantCatalogDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LatinName { get; set; }
    public string Emoji { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Rarity { get; set; } = string.Empty;
    public string LightPreference { get; set; } = string.Empty;
    public string WaterNeeds { get; set; } = string.Empty;
}

/// <summary>
/// DTO for user's plant with all details.
/// </summary>
public class UserPlantDto
{
    public int Id { get; set; }
    public int PlantCatalogId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LatinName { get; set; }
    public string Emoji { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Rarity { get; set; } = string.Empty;
    public string LightPreference { get; set; } = string.Empty;
    public string WaterNeeds { get; set; } = string.Empty;
    public int GrowthStage { get; set; }
    public int GrowthProgress { get; set; }
    public int WaterLevel { get; set; }
    public int SunlightLevel { get; set; }
    public int NutrientLevel { get; set; }
    public bool InGreenhouse { get; set; }
    public DateTime AcquiredAt { get; set; }
    public DateTime? LastCaredAt { get; set; }
}

/// <summary>
/// DTO for plant card in collection grid (minimal data).
/// </summary>
public class UserPlantCardDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;
    public string Rarity { get; set; } = string.Empty;
    public int GrowthStage { get; set; }
    public int WaterLevel { get; set; }
    public int SunlightLevel { get; set; }
    public int NutrientLevel { get; set; }
}

/// <summary>
/// Request DTO for care actions.
/// </summary>
public class CareActionRequest
{
    public CareActionType Action { get; set; }
}

public enum CareActionType
{
    Water = 1,      // 10 coins
    Sunlight = 2,   // 15 coins
    Nutrients = 3,  // 20 coins
    Boost = 4       // 50 coins (all needs + growth speed)
}

/// <summary>
/// Response DTO for care action result.
/// </summary>
public class CareActionResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int CoinsCost { get; set; }
    public int NewWaterLevel { get; set; }
    public int NewSunlightLevel { get; set; }
    public int NewNutrientLevel { get; set; }
    public int NewGrowthStage { get; set; }
    public int NewGrowthProgress { get; set; }
    public bool LeveledUp { get; set; }
}

/// <summary>
/// Response DTO for gacha roll result.
/// </summary>
public class GachaResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserPlantDto? Plant { get; set; }
}

/// <summary>
/// Response DTO for selling a plant.
/// </summary>
public class SellResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int BaseValue { get; set; }
    public int RarityBonus { get; set; }
    public int CareBonus { get; set; }
    public int TotalCoins { get; set; }
}

/// <summary>
/// DTO for greenhouse view with all user's plants.
/// </summary>
public class GreenhouseDto
{
    public List<UserPlantCardDto> Plants { get; set; } = new();
    public int TotalPlants { get; set; }
    public int OvergrownCount { get; set; }
}
