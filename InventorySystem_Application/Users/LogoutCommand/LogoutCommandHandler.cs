using InventorySystem_Application.Common;
using InventorySystem_Domain;
using InventorySystem_Domain.Common;
using MediatR;

namespace InventorySystem_Application.Users.LogoutCommand;

public sealed class LogoutCommandHandler
     : IRequestHandler<LogoutCommand, IResult<bool>>
{
    private readonly IRepository<InventorySystem_Domain.User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    public LogoutCommandHandler(IRepository<InventorySystem_Domain.User> userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<IResult<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByAsync(a => a.UserId == request.UserId);
        if (user == null)
            return Result<bool>.Failure("User details not found");
        user.Logout();
        var isSuccess = await _unitOfWork.ExecuteInTransactionAsync<bool>(async () =>
        {
            var affectedRows = await _unitOfWork.SaveAsync();
            return affectedRows > 0;
        }, cancellationToken);
        return Result<bool>.Success(isSuccess);
    }
}
