namespace KC.Identity.API.Messaging
{
    public record UserInvitation
    {
        public string InviteUserName { get; init; } = "";

        public string InviteUserEmail { get; init; } = "";

        public string ButtonUrl { get; init; } = "";

        public string FirstName { get; init; } = "";

        public string OrgName { get; init; } = "";

        public string OrgAddress { get; init; } = "";

        public string OrgCity { get; init; } = "";

        public string OrgState { get; init; } = "";

        public string OrgZipCode { get; init; } = "";

        public string? OrgPhone { get; init; }
    }
}
