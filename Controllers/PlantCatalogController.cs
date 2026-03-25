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
public class PlantCatalogController : ControllerBase
{
    private readonly PlantContext _context;
    private readonly ILogger<PlantCatalogController> _logger;

    public PlantCatalogController(PlantContext context, ILogger<PlantCatalogController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all plants in the catalog.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlantCatalogDto>>> GetAll()
    {
        var plants = await _context.PlantCatalogs
            .Where(p => p.IsActive)
            .OrderBy(p => p.Rarity)
            .ThenBy(p => p.Name)
            .Select(p => new PlantCatalogDto
            {
                Id = p.Id,
                Name = p.Name,
                LatinName = p.LatinName,
                Emoji = p.Emoji,
                Description = p.Description,
                Rarity = p.Rarity.ToString(),
                LightPreference = p.LightPreference.ToString(),
                WaterNeeds = p.WaterNeeds.ToString()
            })
            .ToListAsync();

        return Ok(plants);
    }

    /// <summary>
    /// Get a specific plant from the catalog.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PlantCatalogDto>> GetById(int id)
    {
        var plant = await _context.PlantCatalogs
            .Where(p => p.Id == id)
            .Select(p => new PlantCatalogDto
            {
                Id = p.Id,
                Name = p.Name,
                LatinName = p.LatinName,
                Emoji = p.Emoji,
                Description = p.Description,
                Rarity = p.Rarity.ToString(),
                LightPreference = p.LightPreference.ToString(),
                WaterNeeds = p.WaterNeeds.ToString()
            })
            .FirstOrDefaultAsync();

        if (plant == null)
            return NotFound("Plant not found in catalog.");

        return Ok(plant);
    }

    /// <summary>
    /// Get plants filtered by rarity.
    /// </summary>
    [HttpGet("rarity/{rarity}")]
    public async Task<ActionResult<IEnumerable<PlantCatalogDto>>> GetByRarity(string rarity)
    {
        if (!Enum.TryParse<PlantRarity>(rarity, true, out var rarityEnum))
            return BadRequest("Invalid rarity. Valid values: Common, Uncommon, Rare, Epic, Legendary");

        var plants = await _context.PlantCatalogs
            .Where(p => p.IsActive && p.Rarity == rarityEnum)
            .OrderBy(p => p.Name)
            .Select(p => new PlantCatalogDto
            {
                Id = p.Id,
                Name = p.Name,
                LatinName = p.LatinName,
                Emoji = p.Emoji,
                Description = p.Description,
                Rarity = p.Rarity.ToString(),
                LightPreference = p.LightPreference.ToString(),
                WaterNeeds = p.WaterNeeds.ToString()
            })
            .ToListAsync();

        return Ok(plants);
    }
}
