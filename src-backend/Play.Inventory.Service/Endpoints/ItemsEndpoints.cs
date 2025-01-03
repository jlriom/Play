
using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Endpoints;

public static class ItemsEndpoints
{
    public static void MapItemsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        const string url = "api/items";
        var itemsGroup = endpoints.MapGroup(url);

        itemsGroup.MapGet($"", async (
            IRepository<InventoryItem> inventoryItemsRepository, 
            IRepository<CatalogItem> catalogItemsRepository, 
            Guid userId ) =>
        {
            if (userId == Guid.Empty)
            {
                return Results.BadRequest();
            }

            var inventoryItemsEntities = (await inventoryItemsRepository.GetAllAsync(item => item.UsedId == userId));
            var itemIds = inventoryItemsEntities.Select(item => item.CatalogItemId);
            var catalogItemsEntities = await catalogItemsRepository.GetAllAsync(item => itemIds.Contains(item.Id));
            
            var inventoryItemDtos = inventoryItemsEntities.Select(item =>
            {
                var catalogItem = catalogItemsEntities.FirstOrDefault(ci => ci.Id == item.CatalogItemId);
                return item.AsDto(catalogItem!.Name, catalogItem.Description);
            });
            
            return Results.Ok(inventoryItemDtos);
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