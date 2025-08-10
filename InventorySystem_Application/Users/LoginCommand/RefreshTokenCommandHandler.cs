using InventorySystem_Application.Common;
using InventorySystem_Application.Users.LoginCommand.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace InventorySystem_Application.Users.LoginCommand;
internal sealed class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, IResult<LoginCommandResponse>>
{
    private readonly IRepository<InventorySystem_Domain.User> _userRepository;
    private readonly IRepository<InventorySystem_Domain.UserRole> _userRoleRepository;
    private readonly IRepository<InventorySystem_Domain.Role> _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    public RefreshTokenCommandHandler(
        IRepository<InventorySystem_Domain.User> userRepository,
        IConfiguration configuration,
        IRepository<InventorySystem_Domain.UserRole> userRoleRepository,
        IUnitOfWork unitOfWork, IRepository<InventorySystem_Domain.Role> roleRepository
        )
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
        _roleRepository = roleRepository;
        _configuration = configuration;
    }
    public async Task<IResult<LoginCommandResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var (Error, Response) = await new LoginAndRefreshToken(_userRepository, _configuration, _userRoleRepository, _unitOfWork, _roleRepository)
            .GetLoginAndRefreshTokenAsync(string.Empty,string.Empty, false, request.RefreshToken, cancellationToken);

        if (Response != null)
        {
            return Result<LoginCommandResponse>.Success(Response);
        }
        else
        {
            return Result<LoginCommandResponse>.Failure(Error);
        }
    }
}
