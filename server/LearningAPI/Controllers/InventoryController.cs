using LearningAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LearningAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InventoryController(MyDbContext context) : ControllerBase
    {
        private readonly MyDbContext _context = context;

        [HttpGet]
        public IActionResult GetAll(string? search)
        {
            IQueryable<Inventory> result = _context.Inventories
                .Include(i => i.Product)
                .Include(i => i.Location);

            if (!string.IsNullOrEmpty(search))
            {
                var trimmedSearch = search.Trim().ToLower();
                result = result.Where(i =>
                    (i.Product != null && i.Product.ProductName.ToLower().Contains(trimmedSearch)) ||
                    (i.Location != null && i.Location.LocationName.ToLower().Contains(trimmedSearch)) ||
                    i.InventoryId.ToString().Contains(trimmedSearch)
                );
            }

            var list = result.OrderByDescending(i => i.CreatedAt).ToList();

            var data = list.Select(i => new
            {
                i.InventoryId,
                i.ProductId,
                ProductName = i.Product?.ProductName,
                i.Quantity,
                i.CreatedAt,
                i.UpdatedAt,
                i.HarvestDate,
                i.ExpiryDate,
                i.LocationId,
                LocationName = i.Location?.LocationName
            });

            return Ok(data);
        }

        [HttpGet("{InventoryId}")]
        public IActionResult GetInventory(int InventoryId)
        {
            var inventory = _context.Inventories
                .Include(i => i.Product)
                .Include(i => i.Location)
                .SingleOrDefault(i => i.InventoryId == InventoryId);

            if (inventory == null) return NotFound();

            return Ok(new
            {
                inventory.InventoryId,
                inventory.ProductId,
                ProductName = inventory.Product?.ProductName,
                inventory.Quantity,
                inventory.CreatedAt,
                inventory.UpdatedAt,
                inventory.HarvestDate,
                inventory.ExpiryDate,
                inventory.LocationId,
                LocationName = inventory.Location?.LocationName
            });
        }

        [HttpPost, Authorize]
        public IActionResult AddInventory(Inventory inventory)
        {
            var product = _context.Products.Find(inventory.ProductId);
            if (product == null)
            {
                return BadRequest("Product does not exist.");
            }

            var location = _context.Locations.Find(inventory.LocationId);
            if (location == null)
            {
                return BadRequest("Location does not exist.");
            }

            int userId = GetUserId();
            var now = DateTime.Now;

            var newInventory = new Inventory
            {
                ProductId = inventory.ProductId,
                Quantity = inventory.Quantity,
                HarvestDate = inventory.HarvestDate,
                ExpiryDate = inventory.ExpiryDate,
                LocationId = inventory.LocationId,
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.Inventories.Add(newInventory);
            _context.SaveChanges();

            return Ok(newInventory);
        }

        [HttpPut("{InventoryId}"), Authorize]
        public IActionResult UpdateInventory(int InventoryId, Inventory inventory)
        {
            var existing = _context.Inventories.Find(InventoryId);
            if (existing == null) return NotFound();

            // check if product exists
            var product = _context.Products.Find(inventory.ProductId);
            if (product == null) return BadRequest("Product does not exist.");


            var location = _context.Locations.Find(inventory.LocationId);
            if (location == null) return BadRequest("Location does not exist.");
            

            existing.ProductId = inventory.ProductId;
            existing.Quantity = inventory.Quantity;
            existing.HarvestDate = inventory.HarvestDate;
            existing.ExpiryDate = inventory.ExpiryDate;
            existing.LocationId = inventory.LocationId;
            existing.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            return Ok(existing);
        }

        [HttpDelete, Authorize]
        public IActionResult DeleteInventory([FromBody] int[] inventoryIds)
        {
            if (inventoryIds == null || inventoryIds.Length == 0)
                return BadRequest("No inventory IDs provided.");

            // Get all inventory items that match the IDs
            var inventories = _context.Inventories
                .Where(i => inventoryIds.Contains(i.InventoryId))
                .ToList();

            if (inventories.Count == 0)
                return NotFound("No inventory items found for the provided IDs.");

            _context.Inventories.RemoveRange(inventories);
            _context.SaveChanges();

            return Ok(new { deletedCount = inventories.Count });
        }

        private int GetUserId()
        {
            return Convert.ToInt32(User.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value)
                .SingleOrDefault());
        }
    }

    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly MyDbContext _context;
        public ProductController(MyDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _context.Products
                .Select(p => new { p.ProductId, p.ProductName, p.ProductImg })
                .ToList();
            return Ok(products);
        }
    }

    [ApiController]
    [Route("[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly MyDbContext _context;
        public LocationController(MyDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll()
        {
            var locations = _context.Locations
                .Select(l => new { l.LocationId, l.LocationName })
                .ToList();
            return Ok(locations);
        }
    }
}
