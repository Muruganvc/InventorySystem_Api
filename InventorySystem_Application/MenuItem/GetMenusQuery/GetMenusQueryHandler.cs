using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.MenuItem.GetMenusQuery;


internal sealed class GetMenusQueryHandler
    : IRequestHandler<GetMenusQuery, IResult<IReadOnlyList<GetMenusQueryResponse>>>
{
    private readonly IRepository<InventorySystem_Domain.MenuItem> _menuItemRepository;

    public GetMenusQueryHandler(
        IRepository<InventorySystem_Domain.MenuItem> menuItemRepository) => _menuItemRepository = menuItemRepository;

    public  async Task<IResult<IReadOnlyList<GetMenusQueryResponse>>> Handle(GetMenusQuery request, CancellationToken cancellationToken)
    {
        var allMenuItems = await _menuItemRepository.Table.ToListAsync(cancellationToken);
        var response = BuildMenuTree(allMenuItems, null);
        return Result<IReadOnlyList<GetMenusQueryResponse>>.Success(response);
    }

    private List<GetMenusQueryResponse> BuildMenuTree(List<InventorySystem_Domain.MenuItem> allItems, int? parentId)
    {
        return allItems
            .Where(item => item.ParentId == parentId)
            .Select(item => new GetMenusQueryResponse(
                item.MenuItemId,
                item.ParentId ?? 0,
                item.OrderBy ?? 0,
                item.Label,
                item.Icon ?? string.Empty,
                item.Route,
                BuildMenuTree(allItems, item.MenuItemId)
            ))
            .ToList();
    }


}
