using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using InventorySystem_Domain;
using MediatR;

namespace InventorySystem_Application.Users.AddOrRemoveUserRoleCommand;
internal class AddOrRemoveUserRoleCommandHandler
    : IRequestHandler<AddOrRemoveUserRoleCommand, IResult<bool>>
{
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddOrRemoveUserRoleCommandHandler(IRepository<UserRole> userRoleRepository, IUnitOfWork unitOfWork)
    {
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<IResult<bool>> Handle(AddOrRemoveUserRoleCommand request, CancellationToken cancellationToken)
    {
        var existingRole = await _userRoleRepository
            .GetByAsync(ur => ur.UserId == request.UserId && ur.RoleId == request.RoleId);

        var result = await _unitOfWork.ExecuteInTransactionAsync<bool>(async () =>
        {
            if (existingRole is null)
            {
                var userRole = UserRole.AddRole(request.UserId, request.RoleId);
                await _userRoleRepository.AddAsync(userRole);
            }
            else
            {
                _userRoleRepository.Delete(existingRole);
            }
            return await _unitOfWork.SaveAsync() > 0;
        }, cancellationToken);
        return Result<bool>.Success(result);
    }
}
