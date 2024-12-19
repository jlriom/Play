using Play.Catalog.Service.Dtos;

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
        
        itemsGroup.MapGet("", () => Items).WithName("GetAllItems");
        itemsGroup.MapGet("{id:guid}", (Guid id) =>
        {
            var item = Items.FirstOrDefault(i => i.Id == id);
            
            return item == null 
                ? Results.NotFound() 
                : Results.Ok(item);
            
        }).WithName("GetById");
        
        itemsGroup.MapPost("", (CreateItemDto createItemDto) =>
        {
            var item = new ItemDto(Guid.NewGuid(), createItemDto.Name, createItemDto.Description, createItemDto.Price, DateTimeOffset.Now);
            Items.Add(item);
            
            return Results.Created($"{url}/{item.Id}", item);
        }).WithName("PostItem");
        
        itemsGroup.MapPut("{id:guid}", (Guid id, UpdateItemDto updateItemDto) =>
        {
            var existingItem = Items.FirstOrDefault(i => i.Id == id);
            
            if (existingItem == null) return Results.NotFound();
            
            var updatedItem = existingItem with
            {
                Name = updateItemDto.Name,
                Description = updateItemDto.Description,
                Price = updateItemDto.Price
            };
            var index = Items.FindIndex(item => item.Id  == id);
            Items[index] = updatedItem;

            return Results.NoContent();
        }).WithName("PutItem");
        
        itemsGroup.MapDelete("{id:guid}", (Guid id) =>
        {
            var index = Items.FindIndex(item => item.Id  == id);
            
            if (index > -1 ) return Results.NotFound();

            Items.RemoveAt(index);
            return Results.NoContent();
        }).WithName("DeleteItem");
    }
}


