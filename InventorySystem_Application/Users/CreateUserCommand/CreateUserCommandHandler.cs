using InventorySystem_Application.Common;
using InventorySystem_Domain;
using InventorySystem_Domain.Common;
using MediatR;

namespace InventorySystem_Application.Users.CreateUserCommand;
internal sealed class CreateUserCommandHandler :
    IRequestHandler<CreateUserCommand, IResult<int>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IRepository<User> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<IResult<int>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var IsExistCompany = await _userRepository.GetByAsync(a => a.UserName == request.UserName);
        if (IsExistCompany != null)
            return Result<int>.Failure("Username already exists.");

        var user = User.Create(request.FirstName, request.LastName, request.UserName, 
            BCrypt.Net.BCrypt.HashPassword(request.Password),
            request.Email, request.MobileNo, 1);

        var userId = await _unitOfWork.ExecuteInTransactionAsync<int>(async () =>
        {
            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveAsync();
            return user.UserId;
        }, cancellationToken);

        return Result<int>.Success(userId);
    }
}
