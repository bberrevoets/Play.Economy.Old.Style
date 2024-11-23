using System.Threading.Tasks;
using MassTransit;
using Play.Catalog.Contracts;
using Play.Common;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers;

public class CatalogItemCreatedConsumer : IConsumer<CatalogItemCreated>
{
    private readonly IRepository<CatalogItem> _repository;

    public CatalogItemCreatedConsumer(IRepository<CatalogItem> repository)
    {
        _repository = repository;
    }

    #region

    public async Task Consume(ConsumeContext<CatalogItemCreated> context)
    {
        var message = context.Message;

        var item = await _repository.GetAsync(message.ItemId);
        if (item != null) return;

        await _repository.CreateAsync(new CatalogItem
            { Id = message.ItemId, Name = message.Name, Description = message.Description });
    }

    #endregion
}