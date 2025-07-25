using InventorySystem_Application.Common;
using InventorySystem_Domain;
using InventorySystem_Domain.Common;
using MediatR;

namespace InventorySystem_Application.Users.PasswordChangeCommand;

internal class PasswordChangeCommandHandler
    : IRequestHandler<PasswordChangeCommand, IResult<bool>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    public PasswordChangeCommandHandler(IRepository<User> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<IResult<bool>> Handle(PasswordChangeCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByAsync(c => c.UserId == request.UserId);

        if (user is null)
            return Result<bool>.Failure("Invalid user name");

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash);

        if (!isPasswordValid)
            return Result<bool>.Failure("Invalid current password");

        user.ChangePassword(BCrypt.Net.BCrypt.HashPassword(request.PasswordHash));
        var isSuccess = await _unitOfWork.ExecuteInTransactionAsync<bool>(async () =>
        {
            var affectedRows = await _unitOfWork.SaveAsync();
            return affectedRows > 0;
        }, cancellationToken);

        return Result<bool>.Success(isSuccess);
    }
}
