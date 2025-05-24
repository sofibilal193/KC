using System;
using System.Collections.Generic;
using KC.Domain.Common.Entities;
using KC.Domain.Common.Events;
using KC.Identity.API.DomainEvents;

namespace KC.Identity.API.Entities
{
    public class User : SqlEntity
    {
        public string? AuthProvider { get; private set; }

        public string? AuthProviderId { get; private set; }

        public string FirstName { get; private set; } = "";

        public string LastName { get; private set; } = "";

        public string Email { get; private set; } = "";

        public string? MobilePhone { get; private set; }

        private DateTime? _lastLoginDateTimeUtc;
        public DateTime? LastLoginDateTimeUtc => _lastLoginDateTimeUtc;

        private DateTime? _lastLogoutDateTimeUtc;
        public DateTime? LastLogoutDateTimeUtc => _lastLogoutDateTimeUtc;

        // DDD Patterns comment
        // Using a private collection field, better for DDD Aggregate's encapsulation
        // so Orgs cannot be added from "outside the AggregateRoot" directly to the collection,
        // but only through the method UserAggregateRoot.AddUser() which includes behaviour.
        private readonly List<OrgUser> _orgs;
        public IReadOnlyCollection<OrgUser> Orgs => _orgs.AsReadOnly();

        protected User()
            : base()
        {
            _orgs = new List<OrgUser>();
        }

        public User(string? authProvider, string? authProviderId, string firstName,
            string lastName, string email, string? mobilePhone)
            : this()
        {
            AuthProvider = authProvider;
            AuthProviderId = authProviderId;
            FirstName = firstName;
            LastName = lastName;
            Email = email.ToLower();
            MobilePhone = mobilePhone;
            AddDomainEvent(new EntityCreatedDomainEvent<User>(this, DefaultOrgId));
        }

        public User(string firstName, string lastName,
            string email, string? mobilePhone)
            : this()
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email.ToLower();
            MobilePhone = mobilePhone;
            AddDomainEvent(new EntityCreatedDomainEvent<User>(this, DefaultOrgId));
        }

        public string FullName
        {
            get
            {
                return string.Concat(FirstName, " ", LastName);
            }
        }

        public bool ContainsDefaultOrg
        {
            get
            {
                return _orgs.Exists(ul => ul.IsDefault);
            }
        }

        public OrgUser? DefaultOrg
        {
            get
            {
                return _orgs.Find(ul => ul.IsDefault);
            }
        }

        public int? DefaultOrgId
        {
            get
            {
                return DefaultOrg?.OrgId;
            }
        }

        public override string ToString()
        {
            return $"{FullName} (Id: {Id})";
        }

        public void Login()
        {
            _lastLoginDateTimeUtc = DateTime.UtcNow;
            AddDomainEvent(new UserLoggedInDomainEvent(Id, DefaultOrgId));
        }

        public void Logout()
        {
            _lastLogoutDateTimeUtc = DateTime.UtcNow;
            AddDomainEvent(new UserLoggedOutDomainEvent(Id, DefaultOrgId));
        }

        public override void Disable()
        {
            base.Disable();
            AddDomainEvent(new EntityDisabledDomainEvent<User>(this, DefaultOrgId));
        }

        public void PwdChanged(string? userName)
        {
            AddDomainEvent(new UserPwdChangedDomainEvent(Id, DefaultOrgId, userName ?? FullName, Email));
        }

        // DDD Patterns comment
        // This Location AggregateRoot's method "UpsertOrg()" should be the only way to add/update users to the Org,
        // so any behavior and validations are controlled by the AggregateRoot
        // in order to maintain consistency between the whole Aggregate.
        public void UpsertOrg(int orgId, int roleId, bool isDefault,
            string? title, bool sendInvite = false)
        {
            var orgUser = _orgs.Find(o => o.OrgId == orgId);
            if (orgUser is null)
            {
                orgUser = new OrgUser(this, orgId, roleId, isDefault, title);
                if (sendInvite)
                    orgUser.Invite();
                _orgs.Add(orgUser);
                AddDomainEvent(new EntityCreatedDomainEvent<OrgUser>(orgUser, DefaultOrgId));
            }
            else
            {
                orgUser.Update(roleId, isDefault, title);
                if (sendInvite)
                    orgUser.Invite();
                AddDomainEvent(new EntityUpdatedDomainEvent<OrgUser>(orgUser, DefaultOrgId));
            }
        }

        public void UpsertOrg(int orgId, int roleId, string? title, bool isInvited)
        {
            var orgUser = _orgs.Find(o => o.OrgId == orgId);
            if (orgUser is null)
            {
                orgUser = new OrgUser(this, orgId, roleId, !ContainsDefaultOrg, title);
                if (isInvited)
                    orgUser.Invite();
                _orgs.Add(orgUser);
                AddDomainEvent(new EntityCreatedDomainEvent<OrgUser>(orgUser, DefaultOrgId));
            }
            else
            {
                orgUser.Update(roleId, title);
                if (isInvited)
                    orgUser.Invite();
                AddDomainEvent(new EntityUpdatedDomainEvent<OrgUser>(orgUser, DefaultOrgId));
            }
        }

        public void DeleteOrg(int orgId, int userId)
        {
            var orgUser = _orgs.Find(o => o.OrgId == orgId && o.UserId == userId);
            if (orgUser is not null)
            {
                _orgs.Remove(orgUser);
            }
        }

        public void Update(string? authProvider, string? authProviderId, string firstName,
            string lastName, string email, string? mobilePhone)
        {
            AuthProvider = authProvider;
            AuthProviderId = authProviderId;
            FirstName = firstName;
            LastName = lastName;
            Email = email.ToLower();
            MobilePhone = mobilePhone;
            AddDomainEvent(new EntityUpdatedDomainEvent<User>(this, DefaultOrgId));
        }

        public void Update(string firstName, string lastName,
            string email, string? mobilePhone)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email.ToLower();
            MobilePhone = mobilePhone;
            AddDomainEvent(new EntityUpdatedDomainEvent<User>(this, DefaultOrgId));
        }

        public void ProcessUserInvitation(string? authProvider, string? authProviderId)
        {
            AuthProvider = authProvider;
            AuthProviderId = authProviderId;
            _lastLoginDateTimeUtc = DateTime.UtcNow;

            // set invites to processed
            foreach (var org in _orgs)
            {
                org.ProcessInvite();
            }
        }

        public void UpdateMobilePhone(string mobilePhone)
        {
            MobilePhone = mobilePhone;
        }
    }
}
