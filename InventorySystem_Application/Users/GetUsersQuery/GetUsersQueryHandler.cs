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
    private readonly IUserInfo _userInfo;
    public GetUsersQueryHandler(IRepository<User> userRepository, IMapper mapper, IUserInfo userInfo)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _userInfo = userInfo;
    }
    public async Task<IResult<IReadOnlyList<GetUsersQueryResponse>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await _userRepository.Table
           .AsNoTracking().Where(w=>w.UserId != _userInfo.UserId)
        .ProjectTo<GetUsersQueryResponse>(_mapper.ConfigurationProvider)
        .OrderBy(u => u.FirstName)
        .ToListAsync(cancellationToken);
        return Result<IReadOnlyList<GetUsersQueryResponse>>.Success(users);
    }
}
