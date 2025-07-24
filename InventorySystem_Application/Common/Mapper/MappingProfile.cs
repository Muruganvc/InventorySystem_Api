using AutoMapper;
using InventorySystem_Domain.Common;
using System.Reflection;

namespace InventorySystem_Application.Common.Mapper;

public static class MappingExtensions
{
    public static void ApplyMappingsFromAssembly(this Profile profile, Assembly assembly)
    {
        var types = assembly.GetExportedTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType &&
                (i.GetGenericTypeDefinition() == typeof(IMapFrom<>) ||
                 i.GetGenericTypeDefinition() == typeof(IMapTo<>))))
            .ToList();

        foreach (var type in types)
        {
            try
            {
                var instance = Activator.CreateInstance(type);
                var mappingMethod = type.GetMethod("Mapping")
                                  ?? type.GetInterface("IMapFrom`1")?.GetMethod("Mapping")
                                  ?? type.GetInterface("IMapTo`1")?.GetMethod("Mapping");

                mappingMethod?.Invoke(instance, new object[] { profile });
            }
            catch (MissingMethodException)
            {
                // Skip types without a parameterless constructor (e.g., handlers, services)
                continue;
            }
        }
    }
}


public class MappingProfile : Profile
{
    public MappingProfile()
    {
        this.ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
