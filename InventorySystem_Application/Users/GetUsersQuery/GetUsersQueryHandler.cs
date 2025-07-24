using AutoMapper;
using AutoMapper.QueryableExtensions;
using InventorySystem_Application.Common;
using InventorySystem_Domain;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.Users.GetUsersQuery;
public sealed class GetUsersQueryHandler
    : IRequestHandler<GetUsersQuery, IResult<IReadOnlyList<GetUsersQueryResponse>>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IMapper _mapper;
    public GetUsersQueryHandler(IRepository<User> userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    public async Task<IResult<IReadOnlyList<GetUsersQueryResponse>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await _userRepository.Table
        .ProjectTo<GetUsersQueryResponse>(_mapper.ConfigurationProvider)
        .OrderBy(u => u.FirstName)
        .ToListAsync(cancellationToken);
        return Result<IReadOnlyList<GetUsersQueryResponse>>.Success(users);
    }
}
