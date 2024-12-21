
using Play.Common;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Endpoints;

public static class ItemsEndpoints
{
    public static void MapItemsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        const string url = "api/items";
        var itemsGroup = endpoints.MapGroup(url);

        itemsGroup.MapGet($"", async (IRepository<InventoryItem> itemsRepository, Guid userId ) =>
        {
            if (userId == Guid.Empty)
            {
                return Results.BadRequest();
            }
            
            var items = (await itemsRepository.GetAllAsync(item => item.UsedId == userId)).Select(item => item.AsDto());

            return Results.Ok(items);
        });

        itemsGroup.MapPost("", async (IRepository<InventoryItem> itemsRepository, GrantItemsDto grantItemsDto) =>
        {
            var inventoryItem = await itemsRepository.GetAsync(item => item.UsedId == grantItemsDto.UserId && item.CatalogItemId == grantItemsDto.CatalogItemId);

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    UsedId = grantItemsDto.UserId,
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    Quantity = grantItemsDto.Quantity,
                    AdquireDate = DateTimeOffset.UtcNow
                };
                await itemsRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemsDto.Quantity;
                await itemsRepository.UpdateAsync(inventoryItem);
            }
            return Results.Ok();
        }).WithName("PostItem");
    }
}