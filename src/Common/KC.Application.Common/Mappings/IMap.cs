using AutoMapper;

#pragma warning disable S2326 // Type that type is mapped from.
namespace KC.Application.Common.Mappings
{
    /// <summary>
    /// Indicates that the type defines custom mappings.
    /// </summary>
    public interface IMap
    {
        void Mapping(Profile profile);
    }

    /// <summary>
    /// Indicates that the type maps one-to-one from <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type that type is mapped from.</typeparam>
    public interface IMapFrom<T> { }

    /// <summary>
    /// Indicates that the type maps one-to-one to <typeparamref name="T"/> (Reverse map).
    /// </summary>
    /// <typeparam name="T">Type that type is mapped to.</typeparam>
    public interface IMapTo<T> { }
}

#pragma warning restore S2326 // Type that type is mapped from.
