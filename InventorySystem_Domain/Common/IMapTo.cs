using AutoMapper;

namespace InventorySystem_Domain.Common;
public interface IMapTo<T>
{
    void Mapping(Profile profile) => profile.CreateMap(GetType(), typeof(T));
}

