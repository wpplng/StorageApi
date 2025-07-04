﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StorageApi.Data;
using StorageApi.DTOs;
using StorageApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StorageApi.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly StorageApiContext _context;

        public ProductsController(StorageApiContext context)
        {
            _context = context;
        }

        // GET: api/Products
        // GET: api/Products?category=categoryName
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProduct([FromQuery] string? category)
        {
            var query = _context.Product.AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category == category);
            }

            var products = await query
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Count = p.Count,
                })
                .ToListAsync();

            return Ok(products);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _context.Product
                .Where(p => p.Id == id)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Count = p.Count,
                })
                .FirstOrDefaultAsync();

            if (product == null) return NotFound();

            return Ok(product);
        }

        // GET: api/Products/stats
        [HttpGet("stats")]
        public async Task<ActionResult<ProductStatsDto>> GetProductStats()
        {
            var products = await _context.Product.ToListAsync();

            if (products.Count == 0)
            {
                return Ok(new ProductStatsDto
                {
                    TotalCount = 0,
                    TotalInventoryValue = 0,
                    AveragePrice = 0
                });
            }

            var stats = new ProductStatsDto
            {
                TotalCount = products.Count,
                TotalInventoryValue = products.Sum(p => p.Price * p.Count),
                AveragePrice = Math.Round(products.Average(p => p.Price), 2) // Rounding to 2 decimal
            };

            return Ok(stats);
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // Ev skapa en update DTO
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, CreateProductDto dto)
        {
            var product = await _context.Product.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.Category = dto.Category;
            product.Shelf = dto.Shelf;
            product.Count = dto.Count;
            product.Description = dto.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProductDto>> PostProduct(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Category = dto.Category,
                Shelf = dto.Shelf,
                Count = dto.Count,
                Description = dto.Description
            };

            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Count = product.Count
            };

            return CreatedAtAction(nameof(GetProduct), new { id = productDto.Id }, productDto);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);

            if (product == null) return NotFound();

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
