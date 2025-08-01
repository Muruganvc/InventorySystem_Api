﻿using InventorySystem_Application.Common;
using InventorySystem_Domain;
using InventorySystem_Domain.Common;
using MediatR;

namespace InventorySystem_Application.Users.UpdateUserCommand;
internal sealed class UpdateUserCommandHandler
    : IRequestHandler<UpdateUserCommand, IResult<bool>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserInfo _userInfo;
    public UpdateUserCommandHandler(IRepository<User> userRepository, IUnitOfWork unitOfWork, IUserInfo userInfo)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _userInfo = userInfo;
    }
    public async Task<IResult<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByAsync(a => a.UserId == request.UserId);
        if (user == null)
            return Result<bool>.Failure("User details not found");

        user.Update(request.FirstName, request.LastName, request.Email, request.MobileNo, request.ImageData, _userInfo.UserId);
        var isSuccess = await _unitOfWork.ExecuteInTransactionAsync<bool>(async () =>
        {
            var affectedRows = await _unitOfWork.SaveAsync();
            return affectedRows > 0;
        }, cancellationToken);

        return Result<bool>.Success(isSuccess);
    }
}
