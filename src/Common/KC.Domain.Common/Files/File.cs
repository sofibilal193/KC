namespace KC.Domain.Common.Files
{
    public readonly record struct File(string Name, byte[]? Content);
}
