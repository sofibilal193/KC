using ProtoBuf;

namespace KC.Config.API.Application
{
    [Serializable]
    [ProtoContract]
    public record ThemesDto
    {
        [ProtoMember(1)]
        public IList<ThemeDto> Themes { get; init; } = new List<ThemeDto>();

        [ProtoMember(2)]
        public string DefaultThemeCode { get; init; } = "";

        [ProtoMember(3)]
        public string Logo { get; init; } = "";
    }

    public readonly record struct ThemeDto(string Code, string Name, string Description, string Icon);
}
