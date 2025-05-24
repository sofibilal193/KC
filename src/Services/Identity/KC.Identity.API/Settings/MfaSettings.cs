using System.Collections.Generic;

namespace KC.Identity.API
{
    public record MfaSettings
    {
        public IList<string>? ExcludedEmails { get; init; }
    }
}
