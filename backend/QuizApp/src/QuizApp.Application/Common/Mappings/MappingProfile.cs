using AutoMapper;
using System.Reflection;

namespace QuizApp.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        var mapPairs = new HashSet<(Type Source, Type Destination)>();

        foreach (var type in assembly.GetExportedTypes())
        {
            if (type.IsAbstract || type.IsInterface) continue;

            var interfaces = type.GetInterfaces();

            foreach (var i in interfaces.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
            {
                var src = i.GetGenericArguments()[0];
                mapPairs.Add((src, type));
            }

        }

        foreach (var (src, dst) in mapPairs)
        {
            CreateMap(src, dst);
        }

    }

}
