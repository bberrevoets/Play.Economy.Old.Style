using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers;

[Route("[controller]")]
[ApiController]
public class ItemsController : ControllerBase
{
    private readonly IRepository<InventoryItem> _itemsRepository;

    public ItemsController(IRepository<InventoryItem> itemsRepository)
    {
        _itemsRepository = itemsRepository;
    }

    [HttpGet]
    [Route("{userId:guid}")]
    public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
    {
        if (userId == Guid.Empty) return BadRequest();

        var items = (await _itemsRepository.GetAllAsync(item => item.UserId == userId)).Select(item => item.AsDto());

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
    {
        var inventoryItem = await _itemsRepository.GetAsync(item =>
            item.UserId == grantItemsDto.UserId && item.CatalogId == grantItemsDto.CatalogItemId);

        if (inventoryItem is null)
        {
            inventoryItem = new InventoryItem
            {
                UserId = grantItemsDto.UserId,
                CatalogId = grantItemsDto.CatalogItemId,
                Quantity = grantItemsDto.Quantity,
                AcquiredDate = DateTimeOffset.UtcNow,
                Id = Guid.CreateVersion7()
            };
            await _itemsRepository.CreateAsync(inventoryItem);
        }
        else
        {
            inventoryItem.Quantity += grantItemsDto.Quantity;
            await _itemsRepository.UpdateAsync(inventoryItem);
        }

        return Ok();
    }
}