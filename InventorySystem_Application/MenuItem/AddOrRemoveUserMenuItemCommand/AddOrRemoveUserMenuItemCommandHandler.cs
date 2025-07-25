using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using InventorySystem_Domain;
using MediatR;

namespace InventorySystem_Application.MenuItem.AddOrRemoveUserMenuItemCommand;
internal sealed class AddOrRemoveUserMenuItemCommandHandler
    : IRequestHandler<AddOrRemoveUserMenuItemCommand, IResult<bool>>
{
    private readonly IRepository<UserMenuPermission> _userMenuRepository;
    private readonly IRepository<InventorySystem_Domain.MenuItem> _menuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddOrRemoveUserMenuItemCommandHandler(IRepository<UserMenuPermission> userMenuRepository, IUnitOfWork unitOfWork,
        IRepository<InventorySystem_Domain.MenuItem> menuRepository)
    {
        _userMenuRepository = userMenuRepository;
        _unitOfWork = unitOfWork;
        _menuRepository = menuRepository;
    }
    public async Task<IResult<bool>> Handle(AddOrRemoveUserMenuItemCommand request, CancellationToken cancellationToken)
    {
        var existingRole = await _userMenuRepository.GetByAsync(u => u.UserId == request.UserId && u.MenuItemId == request.MenuId);

        var menuItem = await _menuRepository.GetByAsync(a => a.MenuItemId == request.MenuId);
        int orderBy = 0;
        if (menuItem is not null)
            orderBy = menuItem.OrderBy ?? 0;

        var result = await _unitOfWork.ExecuteInTransactionAsync<bool>(async () =>
        {
            if (existingRole is null)
            {
                var userRole = UserMenuPermission.AddUserMenu(request.UserId, request.MenuId, orderBy);
                await _userMenuRepository.AddAsync(userRole);
            }
            else
            {
                _userMenuRepository.Delete(existingRole);
            }
            return await _unitOfWork.SaveAsync() > 0;
        }, cancellationToken);
        return Result<bool>.Success(result);
    }
}