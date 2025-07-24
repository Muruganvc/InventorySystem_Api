using AutoMapper;

namespace InventorySystem_Domain.Common;

public interface IMapFrom<T>
{
    void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
}
