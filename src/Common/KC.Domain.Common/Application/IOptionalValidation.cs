namespace KC.Domain.Common.Application
{
    public interface IOptionalValidation
    {
        bool IsValidationDisabled { get; }
    }
}
