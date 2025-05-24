using System.Text.Json.Serialization;

namespace KC.Domain.Common.Config.Commands
{
    /// <summary>
    /// Menucolumn organization configuration information.
    /// </summary>
    public record UpsertMenuConfigCommand
    {
        [JsonIgnore]
        public int OrgId { get; private set; }

        /// <summary>
        /// Value of menucolumn for organization configuration.
        /// </summary>
        public string Value { get; init; } = "";

        public void SetIds(int orgId)
        {
            OrgId = orgId;
        }
    }
}
