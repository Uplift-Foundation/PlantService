using System.Security.Claims;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlantService.Data;
using PlantService.Models;

namespace PlantService.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Authorize]
public class UserPlantController : ControllerBase
{
    private readonly PlantContext _context;
    private readonly ILogger<UserPlantController> _logger;
    private static readonly Random _random = new();

    // Care action costs
    private const int WaterCost = 10;
    private const int SunlightCost = 15;
    private const int NutrientsCost = 20;
    private const int BoostCost = 50;

    // Growth thresholds per stage
    private static readonly int[] GrowthThresholds = { 0, 100, 250, 450, 700 };

    // Sell values by stage
    private static readonly int[] SellBaseValues = { 50, 100, 200, 350, 500 };

    // Rarity multipliers for sell bonus
    private static readonly Dictionary<PlantRarity, double> RarityMultipliers = new()
    {
        { PlantRarity.Common, 1.0 },
        { PlantRarity.Uncommon, 1.25 },
        { PlantRarity.Rare, 1.5 },
        { PlantRarity.Epic, 2.0 },
        { PlantRarity.Legendary, 3.0 }
    };

    public UserPlantController(PlantContext context, ILogger<UserPlantController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private string? GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    /// <summary>
    /// Get all plants owned by the current user.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserPlantCardDto>>> GetMyPlants()
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var plants = await _context.UserPlants
            .Include(up => up.PlantCatalog)
            .Where(up => up.UserId == userId)
            .OrderByDescending(up => up.AcquiredAt)
            .Select(up => new UserPlantCardDto
            {
                Id = up.Id,
                Name = up.PlantCatalog!.Name,
                Emoji = up.PlantCatalog.Emoji,
                Rarity = up.PlantCatalog.Rarity.ToString(),
                GrowthStage = up.GrowthStage,
                WaterLevel = up.WaterLevel,
                SunlightLevel = up.SunlightLevel,
                NutrientLevel = up.NutrientLevel
            })
            .ToListAsync();

        return Ok(plants);
    }

    /// <summary>
    /// Get a specific plant owned by the user.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserPlantDto>> GetPlant(int id)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var plant = await _context.UserPlants
            .Include(up => up.PlantCatalog)
            .Where(up => up.Id == id && up.UserId == userId)
            .Select(up => new UserPlantDto
            {
                Id = up.Id,
                PlantCatalogId = up.PlantCatalogId,
                Name = up.PlantCatalog!.Name,
                LatinName = up.PlantCatalog.LatinName,
                Emoji = up.PlantCatalog.Emoji,
                Description = up.PlantCatalog.Description,
                Rarity = up.PlantCatalog.Rarity.ToString(),
                LightPreference = up.PlantCatalog.LightPreference.ToString(),
                WaterNeeds = up.PlantCatalog.WaterNeeds.ToString(),
                GrowthStage = up.GrowthStage,
                GrowthProgress = up.GrowthProgress,
                WaterLevel = up.WaterLevel,
                SunlightLevel = up.SunlightLevel,
                NutrientLevel = up.NutrientLevel,
                InGreenhouse = up.InGreenhouse,
                AcquiredAt = up.AcquiredAt,
                LastCaredAt = up.LastCaredAt
            })
            .FirstOrDefaultAsync();

        if (plant == null)
            return NotFound("Plant not found or does not belong to you.");

        return Ok(plant);
    }

    /// <summary>
    /// Get greenhouse view with all plants.
    /// </summary>
    [HttpGet("greenhouse")]
    public async Task<ActionResult<GreenhouseDto>> GetGreenhouse()
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var plants = await _context.UserPlants
            .Include(up => up.PlantCatalog)
            .Where(up => up.UserId == userId && up.InGreenhouse)
            .OrderBy(up => up.GrowthStage)
            .ThenByDescending(up => up.AcquiredAt)
            .Select(up => new UserPlantCardDto
            {
                Id = up.Id,
                Name = up.PlantCatalog!.Name,
                Emoji = up.PlantCatalog.Emoji,
                Rarity = up.PlantCatalog.Rarity.ToString(),
                GrowthStage = up.GrowthStage,
                WaterLevel = up.WaterLevel,
                SunlightLevel = up.SunlightLevel,
                NutrientLevel = up.NutrientLevel
            })
            .ToListAsync();

        return Ok(new GreenhouseDto
        {
            Plants = plants,
            TotalPlants = plants.Count,
            OvergrownCount = plants.Count(p => p.GrowthStage == 5)
        });
    }

    /// <summary>
    /// Roll gacha for a new plant.
    /// </summary>
    [HttpPost("gacha")]
    public async Task<ActionResult<GachaResultDto>> RollGacha()
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        // Get all active plants with their weights
        var catalogPlants = await _context.PlantCatalogs
            .Where(p => p.IsActive)
            .ToListAsync();

        if (!catalogPlants.Any())
        {
            return Ok(new GachaResultDto
            {
                Success = false,
                Message = "No plants available in the gacha pool."
            });
        }

        // Weighted random selection
        var totalWeight = catalogPlants.Sum(p => p.GachaWeight);
        var roll = _random.Next(totalWeight);
        var cumulative = 0;
        PlantCatalog? selectedPlant = null;

        foreach (var plant in catalogPlants)
        {
            cumulative += plant.GachaWeight;
            if (roll < cumulative)
            {
                selectedPlant = plant;
                break;
            }
        }

        selectedPlant ??= catalogPlants.Last();

        // Create user plant
        var userPlant = new UserPlant
        {
            UserId = userId,
            PlantCatalogId = selectedPlant.Id,
            GrowthStage = 1,
            WaterLevel = 100,
            SunlightLevel = 100,
            NutrientLevel = 100,
            GrowthProgress = 0,
            InGreenhouse = true,
            AcquiredAt = DateTime.UtcNow
        };

        _context.UserPlants.Add(userPlant);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} rolled gacha and got {PlantName} ({Rarity})",
            userId, selectedPlant.Name, selectedPlant.Rarity);

        return Ok(new GachaResultDto
        {
            Success = true,
            Message = $"You got a {selectedPlant.Rarity} {selectedPlant.Name}!",
            Plant = new UserPlantDto
            {
                Id = userPlant.Id,
                PlantCatalogId = selectedPlant.Id,
                Name = selectedPlant.Name,
                LatinName = selectedPlant.LatinName,
                Emoji = selectedPlant.Emoji,
                Description = selectedPlant.Description,
                Rarity = selectedPlant.Rarity.ToString(),
                LightPreference = selectedPlant.LightPreference.ToString(),
                WaterNeeds = selectedPlant.WaterNeeds.ToString(),
                GrowthStage = userPlant.GrowthStage,
                GrowthProgress = userPlant.GrowthProgress,
                WaterLevel = userPlant.WaterLevel,
                SunlightLevel = userPlant.SunlightLevel,
                NutrientLevel = userPlant.NutrientLevel,
                InGreenhouse = userPlant.InGreenhouse,
                AcquiredAt = userPlant.AcquiredAt,
                LastCaredAt = userPlant.LastCaredAt
            }
        });
    }

    /// <summary>
    /// Care for a plant (water, sunlight, nutrients, or boost).
    /// </summary>
    [HttpPost("{id}/care")]
    public async Task<ActionResult<CareActionResponse>> CarePlant(int id, [FromBody] CareActionRequest request)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var plant = await _context.UserPlants
            .Include(up => up.PlantCatalog)
            .FirstOrDefaultAsync(up => up.Id == id && up.UserId == userId);

        if (plant == null)
            return NotFound("Plant not found or does not belong to you.");

        int cost;
        switch (request.Action)
        {
            case CareActionType.Water:
                cost = WaterCost;
                plant.WaterLevel = Math.Min(100, plant.WaterLevel + 30);
                break;
            case CareActionType.Sunlight:
                cost = SunlightCost;
                plant.SunlightLevel = Math.Min(100, plant.SunlightLevel + 30);
                break;
            case CareActionType.Nutrients:
                cost = NutrientsCost;
                plant.NutrientLevel = Math.Min(100, plant.NutrientLevel + 30);
                break;
            case CareActionType.Boost:
                cost = BoostCost;
                plant.WaterLevel = 100;
                plant.SunlightLevel = 100;
                plant.NutrientLevel = 100;
                plant.GrowthProgress += 25; // Bonus growth
                break;
            default:
                return BadRequest("Invalid care action.");
        }

        // Add growth progress based on care
        var growthGain = request.Action == CareActionType.Boost ? 25 : 10;
        plant.GrowthProgress += growthGain;
        plant.LastCaredAt = DateTime.UtcNow;

        // Check for level up
        var leveledUp = false;
        if (plant.GrowthStage < 5)
        {
            var threshold = GrowthThresholds[plant.GrowthStage];
            if (plant.GrowthProgress >= threshold)
            {
                plant.GrowthStage++;
                leveledUp = true;
                _logger.LogInformation("Plant {PlantId} grew to stage {Stage}", plant.Id, plant.GrowthStage);
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new CareActionResponse
        {
            Success = true,
            Message = leveledUp
                ? $"Your {plant.PlantCatalog!.Name} grew to stage {plant.GrowthStage}!"
                : $"You cared for your {plant.PlantCatalog!.Name}.",
            CoinsCost = cost,
            NewWaterLevel = plant.WaterLevel,
            NewSunlightLevel = plant.SunlightLevel,
            NewNutrientLevel = plant.NutrientLevel,
            NewGrowthStage = plant.GrowthStage,
            NewGrowthProgress = plant.GrowthProgress,
            LeveledUp = leveledUp
        });
    }

    /// <summary>
    /// Sell a plant for coins.
    /// </summary>
    [HttpPost("{id}/sell")]
    public async Task<ActionResult<SellResultDto>> SellPlant(int id)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var plant = await _context.UserPlants
            .Include(up => up.PlantCatalog)
            .FirstOrDefaultAsync(up => up.Id == id && up.UserId == userId);

        if (plant == null)
            return NotFound("Plant not found or does not belong to you.");

        // Calculate sale value
        var baseValue = SellBaseValues[plant.GrowthStage - 1];
        var rarityMultiplier = RarityMultipliers[plant.PlantCatalog!.Rarity];
        var rarityBonus = (int)(baseValue * (rarityMultiplier - 1));

        // Care bonus based on average care levels
        var avgCare = (plant.WaterLevel + plant.SunlightLevel + plant.NutrientLevel) / 3.0;
        var careBonus = (int)(baseValue * (avgCare / 100.0) * 0.25);

        var totalCoins = baseValue + rarityBonus + careBonus;

        // Remove the plant
        _context.UserPlants.Remove(plant);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} sold {PlantName} for {Coins} coins",
            userId, plant.PlantCatalog.Name, totalCoins);

        return Ok(new SellResultDto
        {
            Success = true,
            Message = $"Sold {plant.PlantCatalog.Name} for {totalCoins} coins!",
            BaseValue = baseValue,
            RarityBonus = rarityBonus,
            CareBonus = careBonus,
            TotalCoins = totalCoins
        });
    }

    /// <summary>
    /// Get the sell value preview for a plant (without actually selling).
    /// </summary>
    [HttpGet("{id}/sell-preview")]
    public async Task<ActionResult<SellResultDto>> GetSellPreview(int id)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var plant = await _context.UserPlants
            .Include(up => up.PlantCatalog)
            .FirstOrDefaultAsync(up => up.Id == id && up.UserId == userId);

        if (plant == null)
            return NotFound("Plant not found or does not belong to you.");

        var baseValue = SellBaseValues[plant.GrowthStage - 1];
        var rarityMultiplier = RarityMultipliers[plant.PlantCatalog!.Rarity];
        var rarityBonus = (int)(baseValue * (rarityMultiplier - 1));
        var avgCare = (plant.WaterLevel + plant.SunlightLevel + plant.NutrientLevel) / 3.0;
        var careBonus = (int)(baseValue * (avgCare / 100.0) * 0.25);
        var totalCoins = baseValue + rarityBonus + careBonus;

        return Ok(new SellResultDto
        {
            Success = true,
            Message = "Sell preview",
            BaseValue = baseValue,
            RarityBonus = rarityBonus,
            CareBonus = careBonus,
            TotalCoins = totalCoins
        });
    }

    /// <summary>
    /// Toggle whether a plant is in the greenhouse.
    /// </summary>
    [HttpPatch("{id}/greenhouse")]
    public async Task<ActionResult> ToggleGreenhouse(int id, [FromQuery] bool inGreenhouse)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var plant = await _context.UserPlants
            .FirstOrDefaultAsync(up => up.Id == id && up.UserId == userId);

        if (plant == null)
            return NotFound("Plant not found or does not belong to you.");

        plant.InGreenhouse = inGreenhouse;
        await _context.SaveChangesAsync();

        return Ok(new { plant.Id, plant.InGreenhouse });
    }
}
