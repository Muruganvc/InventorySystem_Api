using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.Users.GetRoleByUserQuery;

internal record GetRoleByUserQueryHandler
    : IRequestHandler<GetRoleByUserQuery, IResult<IReadOnlyList<GetRoleByUserQueryResponse>>>
{
    private readonly IRepository<InventorySystem_Domain.Role> _roleRepository;
    private readonly IRepository<InventorySystem_Domain.UserRole> _userRoleRepository;
    public GetRoleByUserQueryHandler(IRepository<InventorySystem_Domain.Role> roleRepository,
        IRepository<InventorySystem_Domain.UserRole> userRoleRepository)
    {
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
    }
    public async Task<IResult<IReadOnlyList<GetRoleByUserQueryResponse>>> Handle(GetRoleByUserQuery request, CancellationToken cancellationToken)
    {
        var userRoles = await _userRoleRepository.Table
            .AsNoTracking()
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == request.UserId)
            .Select(ur => new GetRoleByUserQueryResponse(
                ur.RoleId,
                ur.UserId,
                ur.RoleId,
                ur.Role.RoleName
            ))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<GetRoleByUserQueryResponse>>.Success(userRoles);
    }
}
