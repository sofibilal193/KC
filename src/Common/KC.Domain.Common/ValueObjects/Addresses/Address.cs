using System.Collections.Generic;
using Newtonsoft.Json;

namespace KC.Domain.Common.ValueObjects.Addresses
{
    public class Address : ValueObject
    {
        public string Type { get; private set; } = "";
        public string Address1 { get; private set; } = "";
        public string? Address2 { get; private set; }
        public string City { get; private set; } = "";
        public string State { get; private set; } = "";
        public string? County { get; private set; }
        public string Country { get; private set; } = "USA";
        public string ZipCode { get; private set; } = "";
        public string? TimeZone { get; private set; }
        public string? GooglePlaceId { get; private set; }

        public string StreetAddress
        {
            get
            {
                return string.Concat(Address1, !string.IsNullOrEmpty(Address2) ? (" " + Address2) : "");
            }
        }

        protected Address() { }

        public Address(string type, string address1, string? address2, string city, string? county, string state, string country, string zipcode, string? timeZone, string? googlePlaceId)
        {
            Type = type;
            Address1 = address1;
            Address2 = address2;
            City = city;
            County = county;
            State = state;
            Country = country;
            ZipCode = zipcode;
            TimeZone = timeZone;
            GooglePlaceId = googlePlaceId;
        }

        public Address(string type, string address1, string? address2, string city, string state, string zipcode, string? timeZone, string? googlePlaceId)
        {
            Type = type;
            Address1 = address1;
            Address2 = address2;
            City = city;
            State = state;
            ZipCode = zipcode;
            TimeZone = timeZone;
            GooglePlaceId = googlePlaceId;
        }

        [JsonConstructor]
        public Address(string type, string address1, string? address2, string city, string state, string zipcode)
        {
            Type = type;
            Address1 = address1;
            Address2 = address2;
            City = city;
            State = state;
            ZipCode = zipcode;
        }

        public Address(string address1, string? address2, string city, string state, string zipcode)
        {
            Address1 = address1;
            Address2 = address2;
            City = city;
            State = state;
            ZipCode = zipcode;
        }

        public void UpdateMissingInfo(Address address)
        {
            Address1 ??= address.Address1;
            Address2 ??= address.Address2;
            City ??= address.City;
            County ??= address.County;
            State ??= address.State;
            Country ??= address.Country;
            ZipCode ??= address.ZipCode;
            TimeZone ??= address.TimeZone;
            GooglePlaceId ??= address.GooglePlaceId;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Type;
            yield return Address1;
            yield return Address2;
            yield return City;
            yield return County;
            yield return State;
            yield return Country;
            yield return ZipCode;
            yield return TimeZone;
            yield return GooglePlaceId;
        }
    }
}
