using System.Collections.Generic;
using KC.Domain.Common.ValueObjects;
using ProtoBuf;

namespace KC.Identity.API.Entities
{
    [Serializable]
    [ProtoContract]
    public class OrgGroup : ValueObject
    {
        [ProtoMember(1)]
        public int ParentOrgId { get; private set; }

        [ProtoMember(2)]
        public int OrgId { get; }

        public OrgGroup() { }

        public OrgGroup(int parentOrgId)
        {
            ParentOrgId = parentOrgId;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return ParentOrgId;
            yield return OrgId;
        }
    }
}
