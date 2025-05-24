using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using AutoMapper;
using KC.Domain.Common.Types;

namespace KC.Application.Common.Mappings
{
    public static class MappingExtensions
    {
        public static void CreateMappings(this Profile profile, Assembly assembly)
        {
            // common mappings
            profile.CreateMap(typeof(PagedList<>), typeof(PagedList<>));

            // dynamic mappings
            foreach (var type in assembly.GetTypes())
            {
                // map types that implement IMap (custom mappings)
                if (type.GetInterface(nameof(IMap)) is not null)
                {
                    var instance = Activator.CreateInstance(type) as IMap;
                    instance?.Mapping(profile);
                }

                // map types that implement IMapFrom (one-to-one mappings)
                foreach (var i in type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                {
                    profile.CreateMap(i.GetGenericArguments().Single(), type);
                }

                // map types that implement IMapTo (one-to-one reverse mappings)
                foreach (var i in type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapTo<>)))
                {
                    profile.CreateMap(type, i.GetGenericArguments().Single());
                }
            }
        }
    }
}
