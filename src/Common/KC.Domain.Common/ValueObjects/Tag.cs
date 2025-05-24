using System.Text.Json.Serialization;
using ProtoBuf;

namespace KC.Domain.Common.ValueObjects
{
    [Serializable]
    [ProtoContract]
    public class Tag : ValueObject
    {
        [ProtoMember(1)]
        public string Name { get; private set; } = "";

        [ProtoMember(2)]
        public string? Value { get; private set; }

        [ProtoMember(3)]
        public DateTime? CreateDateTimeUtc { get; set; }

        [ProtoMember(4)]
        public int? CreateUserId { get; set; }

        [ProtoMember(5)]
        public string? CreateUserName { get; set; }

        [ProtoMember(6)]
        public string? CreateSource { get; set; }

        [ProtoMember(7)]
        public DateTime? ModifyDateTimeUtc { get; set; }

        [ProtoMember(8)]
        public int? ModifyUserId { get; set; }

        [ProtoMember(9)]
        public string? ModifyUserName { get; set; }

        [ProtoMember(10)]
        public string? ModifySource { get; set; }

        protected Tag() { }

        [JsonConstructor]
        public Tag(string name, string? value)
            : this()
        {
            Name = name;
            Value = value;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Name;
            yield return Value;
        }

        public void UpdateValue(string? value)
        {
            Value = value;
        }

        public void UpdateValue(string value, DateTime? modifyDateTimeUtc)
        {
            Value = value;
            ModifyDateTimeUtc = modifyDateTimeUtc;
        }
    }
}
