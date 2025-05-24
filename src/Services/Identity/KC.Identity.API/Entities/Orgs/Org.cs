using System.Collections.Generic;
using System.Linq;
using KC.Domain.Common.Entities;
using KC.Domain.Common.Events;
using KC.Domain.Common.Identity;
using KC.Domain.Common.ValueObjects;
using KC.Domain.Common.ValueObjects.Addresses;

namespace KC.Identity.API.Entities
{
    public class Org : SqlEntity
    {
        public OrgType Type { get; private set; }

        public string? Code { get; private set; }

        public string Name { get; private set; } = string.Empty;

        public string? LegalName { get; private set; }

        public Address? PrimaryAddress => _addresses
            .Find(a => a.Type == OrgAddressTypes.Primary);

        public string? Phone { get; private set; }

        public string? Fax { get; private set; }

        public string? Website { get; private set; }

        public string? TaxId { get; private set; }

        public string? LicenseNo { get; private set; }

        public string? LicenseState { get; private set; }

        // DDD Patterns comment
        // Using a private collection field, better for DDD Aggregate's encapsulation
        // so OrgUsers cannot be added from "outside the AggregateRoot" directly to the collection,
        // but only through the method UserAggregateRoot.AddUser() which includes behaviour.
        private readonly List<Address> _addresses;
        public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

        private readonly List<OrgUser> _users;
        public IReadOnlyCollection<OrgUser> Users => _users.AsReadOnly();

        private readonly List<OrgGroup> _groups;
        public IReadOnlyCollection<OrgGroup> Groups => _groups.AsReadOnly();

        private readonly List<Tag> _tags;
        public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

        #region Constructors

        protected Org()
            : base()
        {
            _users = new List<OrgUser>();
            _addresses = new List<Address>();
            _groups = new List<OrgGroup>();
            _tags = new List<Tag>();
        }

        public Org(OrgType type, string? code,
            string name, string? legalName, Address address,
            string? phone, string? fax, string? website, string? taxId,
            string? licenseNo, string? licenseState, int? parentOrgId)
            : this()
        {
            Type = type;
            Code = code;
            Name = name;
            LegalName = legalName;
            _addresses.Add(address);
            Phone = phone;
            Fax = fax;
            Website = website;
            TaxId = taxId;
            LicenseNo = licenseNo;
            LicenseState = licenseState;
            if (parentOrgId.HasValue)
            {
                _groups.Clear();
                _groups.Add(new OrgGroup(parentOrgId.Value));
            }
            AddDomainEvent(new EntityCreatedDomainEvent<Org>(this));
        }

        #endregion

        #region Methods

        public void Update(OrgType type, string? code,
            string name, string legalName, Address address,
            string? phone, string? fax, string? website, string? taxId,
            string? licenseNo, string? licenseState, int? parentOrgId)
        {
            Type = type;
            Code = code;
            Name = name;
            LegalName = legalName;
            Phone = phone;
            Fax = fax;
            Website = website;
            TaxId = taxId;
            LicenseNo = licenseNo;
            LicenseState = licenseState;
            var primaryAddress = _addresses.Find(a => a.Type == address.Type);
            if (primaryAddress is not null)
            {
                _addresses.Remove(primaryAddress);
            }
            _addresses.Add(address);
            if (parentOrgId.HasValue)
            {
                _groups.Clear();
                _groups.Add(new OrgGroup(parentOrgId.Value));
            }
            AddDomainEvent(new EntityUpdatedDomainEvent<Org>(this));
        }

        public int? GetParentOrgId() => Groups.FirstOrDefault()?.ParentOrgId;

        public void AddParentOrg(int parentOrgId)
        {
            _groups.Add(new OrgGroup(parentOrgId));
        }

        public override void Disable()
        {
            base.Disable();
            AddDomainEvent(new EntityDisabledDomainEvent<Org>(this));
        }

        public override void Enable()
        {
            base.Enable();
            AddDomainEvent(new EntityEnabledDomainEvent<Org>(this));
        }

        // DDD Patterns comment
        // This Location AggregateRoot's method "UpsertUser()" should be the only way to add users to the Location,
        // so any behavior and validations are controlled by the AggregateRoot
        // in order to maintain consistency between the whole Aggregate.
        public void UpsertUser(User user, int roleId, bool isDefault, string? title, bool sendInvite = true)
        {
            var orgUser = _users.Find(o => o.UserId == user.Id);
            if (orgUser is null)
            {
                orgUser = new OrgUser(user, Id, roleId, isDefault, title);
                if (sendInvite)
                    orgUser.Invite();
                _users.Add(orgUser);
                AddDomainEvent(new EntityCreatedDomainEvent<OrgUser>(orgUser, Id));
            }
            else
            {
                orgUser.Update(roleId, isDefault, title);
                if (sendInvite)
                    orgUser.Invite();
                AddDomainEvent(new EntityUpdatedDomainEvent<OrgUser>(orgUser, Id));
            }
        }

        public void DisableUser(OrgUser user)
        {
            var ul = _users.Find(u => u.UserId == user.UserId);
            ul?.Disable();
            AddDomainEvent(new EntityDisabledDomainEvent<OrgUser>(user, Id));
        }

        public void EnableUser(OrgUser user)
        {
            var ul = _users.Find(u => u.UserId == user.UserId);
            ul?.Enable();
            AddDomainEvent(new EntityEnabledDomainEvent<OrgUser>(user, Id));
        }
        #endregion

    }
}
