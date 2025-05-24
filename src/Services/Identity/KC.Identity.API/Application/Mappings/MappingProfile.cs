using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AutoMapper;
using KC.Application.Common.Mappings;

namespace KC.Identity.API.Application
{
    [ExcludeFromCodeCoverage]
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMappings(Assembly.GetExecutingAssembly());
        }
    }
}
