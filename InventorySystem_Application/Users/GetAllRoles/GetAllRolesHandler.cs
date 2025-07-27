using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.Users.GetAllRoles;

internal record class GetAllRolesHandler
    : IRequestHandler<GetRolesQuery, IResult<IReadOnlyList<GetAllRolesResponse>>>
{
    private readonly IRepository<InventorySystem_Domain.Role> _roleRepository;
    public GetAllRolesHandler(IRepository<InventorySystem_Domain.Role> roleRepository) =>
    _roleRepository = roleRepository;
    public async Task<IResult<IReadOnlyList<GetAllRolesResponse>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.Table.AsNoTracking().ToListAsync(cancellationToken);
        var response = roles
            .Select(role => new GetAllRolesResponse(role.RoleId, role.RoleName))
            .ToList();
        return Result<IReadOnlyList<GetAllRolesResponse>>.Success(response);
    }
}
