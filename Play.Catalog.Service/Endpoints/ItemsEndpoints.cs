using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Repositories;

namespace Play.Catalog.Service.Endpoints;

public static class ItemsEndpoints
{
    private static readonly List<ItemDto> Items =
    [
        new(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.Now),
        new(Guid.NewGuid(), "Antidote", "Cures poison", 7, DateTimeOffset.Now),
        new(Guid.NewGuid(), "Bronze sword", "Deals a small amount of damage", 20, DateTimeOffset.Now)
    ];

    
    public static void MapItemsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        const string url = "api/items";
        var itemsGroup = endpoints.MapGroup(url);
        
        itemsGroup.MapGet("", async (ItemsRepository itemsRepository) =>
        {
            var items = (await itemsRepository.GetAllAsync()).Select(item => item.AsDto());
            
            return items;
        }).WithName("GetAllItems");
        
        itemsGroup.MapGet("{id:guid}", async (ItemsRepository itemsRepository, Guid id) =>
        {
            
            var item = await itemsRepository.GetByIdAsync(id);
            
            return item == null 
                ? Results.NotFound() 
                : Results.Ok(item.AsDto());
            
        }).WithName("GetById");
        
        itemsGroup.MapPost("", async (ItemsRepository itemsRepository, CreateItemDto createItemDto) =>
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

            
            return Results.Created($"{url}/{item.Id}", item);
        }).WithName("PostItem");
        
        itemsGroup.MapPut("{id:guid}", async (ItemsRepository itemsRepository, Guid id, UpdateItemDto updateItemDto) =>
        {
            var existingItem = await itemsRepository.GetByIdAsync(id);
            
            if (existingItem == null) return Results.NotFound();
            
            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            return Results.NoContent();
        }).WithName("PutItem");
        
        itemsGroup.MapDelete("{id:guid}", async (ItemsRepository itemsRepository, Guid id) =>
        {
            var existingItem = await itemsRepository.GetByIdAsync(id);
            
            if (existingItem == null) return Results.NotFound();

            await itemsRepository.RemoveAsync(existingItem.Id);
            
            return Results.NoContent();
        }).WithName("DeleteItem");
    }
}


