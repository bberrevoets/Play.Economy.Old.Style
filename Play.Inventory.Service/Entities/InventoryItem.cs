using System;
using Play.Common;

namespace Play.Inventory.Service.Entities;

public class InventoryItem : IEntity
{
    public Guid UserId { get; set; }
    public Guid CatalogId { get; set; }
    public int Quantity { get; set; }
    public DateTimeOffset AcquiredDate { get; set; }
    public Guid Id { get; set; }
}