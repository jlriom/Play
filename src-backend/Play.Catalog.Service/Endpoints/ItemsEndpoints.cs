using MassTransit;
using Play.Catalog.Contracts;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Endpoints;

public static class ItemsEndpoints
{
    public static void MapItemsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        const string url = "api/items";
        var itemsGroup = endpoints.MapGroup(url);

        itemsGroup.MapGet("", async (IRepository<Item> itemsRepository) =>
        {
            
            var items = (await itemsRepository.GetAllAsync()).Select(item => item.AsDto());

            return items;
        }).WithName("GetAllItems");

        itemsGroup.MapGet("{id:guid}", async (
            IRepository<Item> itemsRepository,
            Guid id) =>
        {
            var item = await itemsRepository.GetByIdAsync(id);

            return item == null
                ? Results.NotFound()
                : Results.Ok(item.AsDto());
        }).WithName("GetById");

        itemsGroup.MapPost("", async (
            IRepository<Item> itemsRepository,
            IPublishEndpoint publishEndpoint,
            CreateItemDto createItemDto) =>
            {
                var item = new Item
                {
                    Id = Guid.NewGuid(),
                    Name = createItemDto.Name,
                    Description = createItemDto.Description,
                    Price = createItemDto.Price,
                    CreatedDate = DateTimeOffset.UtcNow
                };

                await itemsRepository.CreateAsync(item);

                await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

                return Results.Created($"{url}/{item.Id}", item);
            }).WithName("PostItem");

        itemsGroup.MapPut("{id:guid}", async (
            IRepository<Item> itemsRepository, 
            IPublishEndpoint publishEndpoint,
            Guid id, 
            UpdateItemDto updateItemDto) =>
            {
                var existingItem = await itemsRepository.GetByIdAsync(id);

                if (existingItem == null) return Results.NotFound();

                existingItem.Name = updateItemDto.Name;
                existingItem.Description = updateItemDto.Description;
                existingItem.Price = updateItemDto.Price;

                await itemsRepository.UpdateAsync(existingItem);

                await publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));

                return Results.NoContent();
            }).WithName("PutItem");

        itemsGroup.MapDelete("{id:guid}", async (
            IRepository<Item> itemsRepository, 
            IPublishEndpoint publishEndpoint,
            Guid id) =>
        {
            var existingItem = await itemsRepository.GetByIdAsync(id);

            if (existingItem == null) return Results.NotFound();

            await itemsRepository.RemoveAsync(existingItem.Id);

            await publishEndpoint.Publish(new CatalogItemDeleted(existingItem.Id));

            return Results.NoContent();
        }).WithName("DeleteItem");
    }
}