namespace InventorySystem_Domain;
public class User
{
    public int UserId { get; set; } // Identity (Primary Key)
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime PasswordLastChanged { get; set; } = DateTime.UtcNow;
    public DateTime PasswordExpiresAt { get; set; } // Computed column
    public bool IsPasswordExpired { get; set; } = false;
    public DateTime? LastLogin { get; set; }
    public string MobileNo { get; set; } = string.Empty;
    public byte[]? ProfileImage { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public int? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public uint RowVersion { get; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserMenuPermission> UserMenuPermissions { get; set; } = new List<UserMenuPermission>();
    public ICollection<OrderItem>? CreatedOrderItems { get; set; }
    public static User Create(string firstName, string? lastName, string userName,
        string password, string email, string mobileNo, int createdBy)
    {
        return new User
        {
            FirstName = firstName,
            LastName = lastName ?? string.Empty,
            UserName = userName,
            Email = email,
            MobileNo = mobileNo,
            PasswordHash = password,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(string firstName, string? lastName, string email,
        string mobileNo, byte[]? profileImage, int modifiedBy)
    {
        FirstName = firstName;
        LastName = lastName ?? null;
        Email = email;
        MobileNo = mobileNo;
        ProfileImage = profileImage;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }
    public void SetActiveStatus(bool isActive, int modifiedBy)
    {
        IsActive = isActive;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public void ChangePassword(string password)
    {
        PasswordHash = password;
        PasswordLastChanged = DateTime.UtcNow;
        IsPasswordExpired = false;
    }

    public void UpdateRefreshToken(string refreshToken)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(30);
    }
}
