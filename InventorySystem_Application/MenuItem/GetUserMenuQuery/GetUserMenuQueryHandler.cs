using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.MenuItem.GetUserMenuQuery;

internal sealed class GetUserMenuQueryHandler
    : IRequestHandler<GetUserMenuQuery, IResult<IReadOnlyList<GetMenuItemQueryResponse>>>
{
    private readonly IRepository<InventorySystem_Domain.UserMenuPermission> _userMenuPermissionRepository;
    private readonly IRepository<InventorySystem_Domain.MenuItem> _menuItemRepository;
    public GetUserMenuQueryHandler(
        IRepository<InventorySystem_Domain.UserMenuPermission> userMenuPermissionRepository,
        IRepository<InventorySystem_Domain.MenuItem> menuItemRepository)
    {
        _userMenuPermissionRepository = userMenuPermissionRepository;
        _menuItemRepository = menuItemRepository;
    }
    public async Task<IResult<IReadOnlyList<GetMenuItemQueryResponse>>> Handle(GetUserMenuQuery request, CancellationToken cancellationToken)
    {
        var menuItems = await _menuItemRepository.Table.AsNoTracking()
            .Join(
                _userMenuPermissionRepository.Table.AsNoTracking(),
                menu => menu.MenuItemId,
                perm => perm.MenuItemId,
                (menu, perm) => new { menu, perm }
            )
            .Where(x => x.perm.UserId == request.UserId)
            .Select(x => x.menu)
            .ToListAsync(cancellationToken);

        var response = BuildMenuTree(menuItems, null);
        return Result<IReadOnlyList<GetMenuItemQueryResponse>>.Success(response.OrderBy(a => a.Id).ToList());
    }

    private List<GetMenuItemQueryResponse> BuildMenuTree(List<InventorySystem_Domain.MenuItem> allItems, int? parentId)
    {
        return allItems
            .Where(m => m.ParentId == parentId)
            .Select(m => new GetMenuItemQueryResponse
            {
                Id = m.MenuItemId,
                ParentId = m.ParentId ?? 0,
                Label = m.Label,
                Icon = m.Icon ?? string.Empty,
                Route = m.Route,
                SubMenuItem = BuildMenuTree(allItems, m.MenuItemId)
            }).OrderBy(x => x.OrderBy)
            .ToList();
    }
}
