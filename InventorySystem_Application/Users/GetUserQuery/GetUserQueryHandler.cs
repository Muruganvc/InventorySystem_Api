using InventorySystem_Application.Category.GetCategoryQuery;
using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.Users.GetUserQuery;

internal sealed class GetUserQueryHandler
    : IRequestHandler<GetUserQuery, IResult<GetUserQueryResponse>>
{
    private readonly IRepository<InventorySystem_Domain.User> _userRepository;

    public GetUserQueryHandler(
        IRepository<InventorySystem_Domain.User> userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<IResult<GetUserQueryResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.Table
            .AsNoTracking()
            .Where(u => u.UserId == request.UserId)
            .Select(u => new GetUserQueryResponse(
                u.UserId,
                u.FirstName,
                u.LastName,
                u.UserName,
                u.Email,
                u.IsActive,
                u.MobileNo,
                u.ProfileImage != null
                    ? $"data:image/jpeg;base64,{Convert.ToBase64String(u.ProfileImage)}"
                    : string.Empty
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return user is null
            ? Result<GetUserQueryResponse>.Failure("User not found.")
            : Result<GetUserQueryResponse>.Success(user);
    }
}