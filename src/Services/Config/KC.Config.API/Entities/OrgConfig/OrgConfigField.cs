using KC.Domain.Common.Config;
using KC.Domain.Common.Entities;
using KC.Domain.Common.Identity;

namespace KC.Config.API.Entities
{
    public class OrgConfigField : SqlEntity
    {
        /// <summary>
        /// Specifies the type of org the field applies to (optional)
        /// </summary>
        public OrgType? OrgType { get; private set; }

        public string Category { get; private set; } = "";

        public FieldType FieldType { get; private set; }

        public string Name { get; private set; } = "";

        public string? Description { get; private set; }

        public short DisplayOrder { get; private set; }

        /// <summary>
        /// The default value
        /// </summary>
        public string? DefaultValue { get; private set; }

        /// <summary>
        /// The minimum length or value
        /// </summary>
        public decimal? MinValue { get; private set; }

        /// <summary>
        /// The maximum length or value
        /// </summary>
        public decimal? MaxValue { get; private set; }

        /// <summary>
        /// The regular expression to use to validate the value
        /// </summary>
        public string? RegexValidator { get; private set; }

        /// <summary>
        /// Indicates whether this field is visible/editable by orgs themselves
        /// </summary>
        public bool IsOrgVisible { get; private set; }

        private readonly List<FieldValue> _values;
        public IReadOnlyCollection<FieldValue> Values => _values.AsReadOnly();

        private readonly List<OrgConfigFieldValue> _orgValues;
        public IReadOnlyCollection<OrgConfigFieldValue> OrgValues => _orgValues.AsReadOnly();

        #region Constructors

        protected OrgConfigField()
        {
            _values = new();
            _orgValues = new();
        }

        public OrgConfigField(OrgType? orgType, string category, FieldType fieldType, string name,
            string? description, short displayOrder, string? defaultValue, decimal? minValue, decimal? maxValue,
            string? regexValidator, bool isOrgVisible, List<FieldValue> fieldValues) : this()
        {
            OrgType = orgType;
            Category = category;
            FieldType = fieldType;
            Name = name;
            Description = description;
            DisplayOrder = displayOrder;
            DefaultValue = defaultValue;
            MinValue = minValue;
            MaxValue = maxValue;
            RegexValidator = regexValidator;
            IsOrgVisible = isOrgVisible;
            _values = fieldValues;
        }

        public OrgConfigField(OrgType? orgType, string category, FieldType fieldType,
            string name, string? description, short displayOrder) : this()
        {
            OrgType = orgType;
            Category = category;
            FieldType = fieldType;
            Name = name;
            Description = description;
            DisplayOrder = displayOrder;
        }
        public OrgConfigField(OrgType? orgType, string category, FieldType fieldType,
          string name, string? description, decimal? minValue, decimal? maxValue, string? regexValidator, List<FieldValue> fieldValues, short displayOrder) : this()
        {
            OrgType = orgType;
            Category = category;
            FieldType = fieldType;
            Name = name;
            Description = description;
            DisplayOrder = displayOrder;
            MinValue = minValue;
            MaxValue = maxValue;
            RegexValidator = regexValidator;
            _values = fieldValues;
        }

        #endregion

        #region Methods

        public void Update(OrgConfigField field)
        {
            OrgType = field.OrgType;
            Category = field.Category;
            FieldType = field.FieldType;
            Name = field.Name;
            Description = field.Description;
            DisplayOrder = field.DisplayOrder;
            DefaultValue = field.DefaultValue;
            MinValue = field.MinValue;
            MaxValue = field.MaxValue;
            RegexValidator = field.RegexValidator;
            IsOrgVisible = field.IsOrgVisible;
            _orgValues.Clear();
            _orgValues.AddRange(field.OrgValues);
            _values.Clear();
            if (field.Values is not null)
            {
                _values.AddRange(field.Values);
            }
        }

        #endregion

    }
}
