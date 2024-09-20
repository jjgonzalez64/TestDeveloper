using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

public class Location
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public Point Coordinates { get; set; }
    public LineString Route { get; set; }
}



[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LocationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateLocation([FromBody] Location location)
    {
        _context.Locations.Add(location);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetLocationById), new { id = location.Id }, location);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLocation(int id, [FromBody] Location updatedLocation)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null) return NotFound();

        location.Name = updatedLocation.Name;
        location.Address = updatedLocation.Address;
        location.Coordinates = updatedLocation.Coordinates;
        location.Route = updatedLocation.Route;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLocation(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null) return NotFound();

        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLocationById(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        return location == null ? NotFound() : Ok(location);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchLocations(string? name, string? address, string? coordinates)
    {
        var query = _context.Locations.AsQueryable();

        if (!string.IsNullOrEmpty(name))
            query = query.Where(l => l.Name.Contains(name));

        if (!string.IsNullOrEmpty(address))
            query = query.Where(l => l.Address.Contains(address));

        if (!string.IsNullOrEmpty(coordinates))
        {
            var point = new Point(Double.Parse(coordinates.Split(',')[0]), Double.Parse(coordinates.Split(',')[1]))
            { SRID = 4326 };
            query = query.Where(l => l.Coordinates.Distance(point) < 1000);  // 1 km radius search
        }

        var result = await query.ToListAsync();
        return Ok(result);
    }
}
