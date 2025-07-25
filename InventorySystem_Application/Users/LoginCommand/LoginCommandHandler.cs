using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InventorySystem_Application.Users.LoginCommand;
internal sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, IResult<LoginCommandResponse>>
{
    private readonly IRepository<InventorySystem_Domain.User> _userRepository;
    private readonly IRepository<InventorySystem_Domain.UserRole> _userRoleRepository;
    private readonly IRepository<InventorySystem_Domain.Role> _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    public LoginCommandHandler(
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
    public async Task<IResult<LoginCommandResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByAsync(u => u.UserName == request.UserName && u.IsActive);
        if (user is null)
            return Result<LoginCommandResponse>.Failure("Invalid user name");

        //if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        //    return Result<LoginCommandResponse>.Failure("Invalid password");

        var userRoles = await _userRoleRepository.GetListByAsync(ur => ur.UserId == user.UserId);
        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

        var roles = await _roleRepository.GetListByAsync(r => roleIds.Contains(r.RoleId));
        var roleNames = roles.Select(r => r.RoleCode).ToList();

        var token = GenerateJwtToken(
            user.UserName,
            user.Email ?? string.Empty,
            roleNames,
            user.UserId
        );

        var companyInfo = await _unitOfWork
            .Repository<InventorySystem_Domain.InventoryCompanyInfo>()
            .Table
            .FirstOrDefaultAsync(cancellationToken);

        GetInventoryCompanyInfoQueryResponse? companyResponse = null;

        if (companyInfo is not null)
        {
            var base64Image = companyInfo.QrCode != null
                ? $"data:image/jpeg;base64,{Convert.ToBase64String(companyInfo.QrCode)}"
                : string.Empty;

            companyResponse = new GetInventoryCompanyInfoQueryResponse(
                InventoryCompanyInfoId: companyInfo.InventoryCompanyInfoId,
                InventoryCompanyInfoName: companyInfo.InventoryCompanyInfoName,
                Description: companyInfo.Description,
                Address: companyInfo.Address,
                MobileNo: companyInfo.MobileNo,
                GstNumber: companyInfo.GstNumber,
                ApiVersion: companyInfo.ApiVersion,
                UiVersion: companyInfo.UiVersion,
                QrCodeBase64: base64Image,
                Email: companyInfo.Email ?? string.Empty,
                BankName: companyInfo.BankName,
                BankBranchName: companyInfo.BankBranchName,
                BankAccountNo: companyInfo.BankAccountNo,
                BankBranchIFSC: companyInfo.BankBranchIFSC
            );
        }

        var response = new LoginCommandResponse(
            UserId: user.UserId,
            FirstName: user.FirstName,
            LastName: user.LastName ?? string.Empty,
            Email: user.Email ?? string.Empty,
            user.UserName,
            Token: token,
            InvCompanyInfo: companyResponse
        );
        return Result<LoginCommandResponse>.Success(response);
    }

    private string GenerateJwtToken(string username, string email, List<string> roleNames, int userId)
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
        foreach (var role in roleNames)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiryMinutes"]!));

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}