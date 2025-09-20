using InventorySystem_Application.Common;
using InventorySystem_Application.InventoryCompanyInfo.GetInventoryCompanyInfoQuery;
using InventorySystem_Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InventorySystem_Application.Users.LoginCommand.Common
{
    internal class LoginAndRefreshToken
    {
        private readonly IRepository<InventorySystem_Domain.User> _userRepository;
        private readonly IRepository<InventorySystem_Domain.UserRole> _userRoleRepository;
        private readonly IRepository<InventorySystem_Domain.Role> _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public LoginAndRefreshToken(
            IRepository<InventorySystem_Domain.User> userRepository,
            IConfiguration configuration,
            IRepository<InventorySystem_Domain.UserRole> userRoleRepository,
            IUnitOfWork unitOfWork,
            IRepository<InventorySystem_Domain.Role> roleRepository)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _unitOfWork = unitOfWork;
            _roleRepository = roleRepository;
            _configuration = configuration;
        }

        public async Task<(string Error, LoginCommandResponse? Response)> GetLoginAndRefreshTokenAsync(
    string userName, string password, bool isLogin, string refreshToken, CancellationToken cancellationToken)
        {
            // Step 1: Handle refresh token logic if login is not required
            if (!isLogin)
            {
                var user = await _userRepository.GetByAsync(u => u.RefreshToken == refreshToken
                    && u.IsActive && u.RefreshTokenExpiry > DateTime.UtcNow);

                if (user == null) return ("Invalid Refresh token", null);

                return await GenerateLoginResponse(user, cancellationToken); // Return if refresh token is valid
            }

            // Step 2: Handle login logic if login is required
            var loginUser = await _userRepository.GetByAsync(u => u.UserName == userName && u.IsActive);
            if (loginUser == null) return ("Invalid username", null);

            if (!BCrypt.Net.BCrypt.Verify(password, loginUser.PasswordHash))
                return ("Invalid password", null);

            return await GenerateLoginResponse(loginUser, cancellationToken); // Generate response for valid login user
        }

        private async Task<(string Error, LoginCommandResponse? Response)> GenerateLoginResponse(
            InventorySystem_Domain.User user, CancellationToken cancellationToken)
        {
            // Fetch roles for the user
            var roleIds = (await _userRoleRepository.GetListByAsync(ur => ur.UserId == user.UserId))
                .Select(ur => ur.RoleId)
                .ToList();

            var roles = await _roleRepository.GetListByAsync(r => roleIds.Contains(r.RoleId));
            var roleNames = roles.Select(r => r.RoleCode).ToList();

            // Generate JWT token
            var token = GenerateJwtToken(user.UserName, user.Email ?? string.Empty, roleNames, user.UserId);

            // Get company info
            var companyInfo = await _unitOfWork
                .Repository<InventorySystem_Domain.InventoryCompanyInfo>()
                .Table
                .FirstOrDefaultAsync(cancellationToken);

            GetInventoryCompanyInfoQueryResponse? companyResponse = companyInfo != null ? new GetInventoryCompanyInfoQueryResponse(
                companyInfo.InventoryCompanyInfoId,
                companyInfo.InventoryCompanyInfoName,
                companyInfo.Description,
                companyInfo.Address,
                companyInfo.MobileNo,
                companyInfo.GstNumber,
                companyInfo.ApiVersion,
                companyInfo.UiVersion,
                companyInfo.QrCode != null ? $"data:image/jpeg;base64,{Convert.ToBase64String(companyInfo.QrCode)}" : string.Empty,
                companyInfo.Email ?? string.Empty,
                companyInfo.BankName,
                companyInfo.BankBranchName,
                companyInfo.BankAccountNo,
                companyInfo.BankBranchIFSC,
                companyInfo.IsActive
            ) : null;

            // Generate response with JWT and refresh token
            var refreshToken = GenerateRefreshToken;
            var response = new LoginCommandResponse(
                user.UserId,
                user.FirstName,
                user.LastName ?? string.Empty,
                user.Email ?? string.Empty,
                user.UserName,
                token,
                companyResponse,
                refreshToken
            );

            // Update user with new refresh token and save
            user.UpdateRefreshToken(refreshToken);
            await _unitOfWork.SaveAsync();

            return ("", response);
        }

        private string GenerateJwtToken(string username, string email, List<string> roleNames, int userId)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, username),
                new(ClaimTypes.Email, email),
                new(ClaimTypes.NameIdentifier, userId.ToString())
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
        private string GenerateRefreshToken => Guid.NewGuid().ToString();
    }
}
