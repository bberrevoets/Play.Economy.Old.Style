using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;

namespace Play.Catalog.Service.Controllers;

[Route("[controller]")]
[ApiController]
public class ItemsController : ControllerBase
{
    private static readonly List<ItemDto> Items =
    [
        new(Guid.CreateVersion7(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
        new(Guid.CreateVersion7(), "Antidote", "Cures poison", 7, DateTimeOffset.UtcNow),
        new(Guid.CreateVersion7(), "Bronze sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow)
    ];

    [HttpGet]
    public IEnumerable<ItemDto> Get()
    {
        return Items;
    }

    [HttpGet]
    [Route("{id:guid}")]
    public ActionResult<ItemDto> GetById(Guid id)
    {
        var item = Items.Find(i => i.Id == id);

        if (item == null) return NotFound();

        return item;
    }

    [HttpPost]
    public ActionResult<ItemDto> Post(CreateItemDto createItemDto)
    {
        var item = new ItemDto(Guid.CreateVersion7(), createItemDto.Name, createItemDto.Description,
            createItemDto.Price, DateTimeOffset.UtcNow);

        Items.Add(item);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:guid}")]
    public IActionResult Put(Guid id, UpdateItemDto updateItemDto)
    {
        var existingItem = Items.Find(i => i.Id == id);

        if (existingItem == null) return NotFound();

        var updatedItem = existingItem with
        {
            Name = updateItemDto.Name,
            Description = updateItemDto.Description,
            Price = updateItemDto.Price
        };

        var index = Items.FindIndex(item => item.Id == id);
        Items[index] = updatedItem;

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var item = Items.Find(i => i.Id == id);

        if (item == null) return NotFound();

        Items.Remove(item);

        return NoContent();
    }
}