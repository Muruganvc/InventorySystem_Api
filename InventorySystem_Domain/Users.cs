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
}
