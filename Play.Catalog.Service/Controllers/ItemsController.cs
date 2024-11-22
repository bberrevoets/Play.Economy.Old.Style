﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Controllers;

[Route("[controller]")]
[ApiController]
public class ItemsController : ControllerBase
{
    private readonly IRepository<Item> _itemsRepository;

    private static int _requestCounter = 0;

    public ItemsController(IRepository<Item> itemsRepository)
    {
        _itemsRepository = itemsRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
    {
        _requestCounter++;
        Console.WriteLine($"Request {_requestCounter}: Starting...");

        if (_requestCounter <= 2)
        {
            Console.WriteLine($"Request {_requestCounter}: Delaying...");
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        if (_requestCounter <= 4)
        {
            Console.WriteLine($"Request {_requestCounter}: 500 (Internal Server Error)...");
            return StatusCode(500);
        }
        
        var items = (await _itemsRepository.GetAllAsync()).Select(item => item.AsDto());
        
        Console.WriteLine($"Request {_requestCounter}: 200 (OK).");
        
        return Ok(items);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
    {
        var item = await _itemsRepository.GetAsync(id);

        if (item == null) return NotFound();

        return Ok(item.AsDto());
    }

    [HttpPost]
    public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
    {
        var item = new Item
        {
            Name = createItemDto.Name,
            Description = createItemDto.Description,
            Price = createItemDto.Price,
            CreatedDate = DateTimeOffset.UtcNow,
            Id = Guid.CreateVersion7()
        };

        await _itemsRepository.CreateAsync(item);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
    {
        var existingItem = await _itemsRepository.GetAsync(id);

        if (existingItem == null) return NotFound();

        existingItem.Name = updateItemDto.Name;
        existingItem.Description = updateItemDto.Description;
        existingItem.Price = updateItemDto.Price;

        await _itemsRepository.UpdateAsync(existingItem);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _itemsRepository.RemoveAsync(id);

        return NoContent();
    }
}