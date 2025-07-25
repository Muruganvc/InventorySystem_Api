using AutoMapper;
using InventorySystem_Domain;
using InventorySystem_Domain.Common;

namespace InventorySystem_Application.Users.GetUsersQuery;
public class GetUsersQueryResponse : IMapFrom<User>
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = default!;
    public string? LastName { get; set; }
    public string UserName { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public DateTime PasswordLastChanged { get; set; }
    public DateTime PasswordExpiresAt { get; set; }
    public bool IsPasswordExpired { get; set; }
    public DateTime? LastLogin { get; set; }
    public string MobileNo { get; set; } = default!;
    public byte[]? ProfileImage { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public uint RowVersion { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<User, GetUsersQueryResponse>();
    }
}


//public record GetUsersQueryResponse(
//    int UserId,
//    string FirstName,
//    string? LastName,
//    string UserName,
//    string PasswordHash,
//    string? Email,
//    bool IsActive,
//    DateTime PasswordLastChanged,
//    DateTime PasswordExpiresAt,
//    bool IsPasswordExpired,
//    DateTime? LastLogin,
//    string MobileNo,
//    byte[]? ProfileImage,
//    int CreatedBy,
//    DateTime CreatedDate,
//    int? ModifiedBy,
//    DateTime? ModifiedDate
//) : IMapFrom<User>
//{
//    public void Mapping(Profile profile)
//    {
//        profile.CreateMap<User, GetUsersQueryResponse>()
//            .ForCtorParam(nameof(UserId), opt => opt.MapFrom(src => src.UserId))
//            .ReverseMap();
//    }
//}



//public class ProductDto : IMapFrom<Product>, IMapTo<Product>
//{
//    public int Id { get; set; }
//    public string Name { get; set; }

//    public void Mapping(Profile profile)
//    {
//        profile.CreateMap<Product, ProductDto>()
//               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProductTitle));

//        profile.CreateMap<ProductDto, Product>()
//               .ForMember(dest => dest.ProductTitle, opt => opt.MapFrom(src => src.Name));
//    }
//}
