using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using InventorySystem_Domain;
using MediatR;

namespace InventorySystem_Application.Users.ForgetPasswordCommand;

internal class ForgetPasswordCommandHandler
    : IRequestHandler<ForgetPasswordCommand, IResult<bool>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    public ForgetPasswordCommandHandler(IRepository<User> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<IResult<bool>> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByAsync(c => c.UserId == request.UserId);
        if (user is null)
            return Result<bool>.Failure("Invalid user name");

        if (user.MobileNo != request.MobileNo)
            return Result<bool>.Failure("Invalid Mobile No.");

        user.ChangePassword(BCrypt.Net.BCrypt.HashPassword(request.Password));
        var isSuccess = await _unitOfWork.ExecuteInTransactionAsync<bool>(async () =>
        {
            var affectedRows = await _unitOfWork.SaveAsync();
            return affectedRows > 0;
        }, cancellationToken);

        return Result<bool>.Success(isSuccess);
    }
}
