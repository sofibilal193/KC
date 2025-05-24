using System.Reflection;
using AutoMapper;
using KC.Application.Common.Mappings;
using KC.Config.API.Entities;
using KC.Domain.Common;
using KC.Domain.Common.ValueObjects;

namespace KC.Config.API.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMappings(Assembly.GetExecutingAssembly());
            Mapping(this);
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<OrgConfigField, OrgConfigDto>()
                .ForMember(d => d.ModifyUser, opt => opt.MapFrom(s => s.ModifyUserName))
                .ForMember(d => d.FieldValues, opt => opt.MapFrom(s => s.Values))
                .ForMember(d => d.Value, opt => opt.MapFrom(s => s.OrgValues.Count == 1 ? (s.OrgValues.First().Value ?? s.DefaultValue) : s.DefaultValue));

            profile.CreateMap<UserConfigField, OrgConfigDto>()
                .ForMember(d => d.ModifyUser, opt => opt.MapFrom(s => s.ModifyUserName))
                .ForMember(d => d.FieldValues, opt => opt.MapFrom(s => s.Values))
                .ForMember(d => d.Value, opt => opt.MapFrom(s => s.UserValues.Count == 1 ? (s.UserValues.First().Value ?? s.DefaultValue) : s.DefaultValue));

            profile.CreateMap<FieldValue, FieldValuesDto>();

            profile.CreateMap<ConfigItem, ConfigItemDto>();



            profile.CreateMap<Tag, TagDto>().ReverseMap();
        }
    }
}
