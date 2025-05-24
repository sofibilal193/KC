using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using KC.Persistence.Common.DataSecurity;
using KC.Persistence.Common.Encryption;
using KC.Persistence.Common.Utils;

namespace KC.Persistence.Common.Extensions
{
    public static class PropertyBuilderExtensions
    {
        /// <summary>
        /// Configures the property so that the property value is converted to and from the
        /// database using <see cref="JsonSerializer">JsonSerializer</see>.
        /// </summary>
        /// <typeparam name="T">The target type of the JSON value.</typeparam>
        /// <param name="propertyBuilder">Property builder API for configuring a Microsoft.EntityFrameworkCore.Metadata.IMutableProperty.</param>
        /// <returns>The same builder instance so that multiple configuration calls can be chained.</returns>
        // Note: restriction is commented out so that it can be used for IReadOnlyCollection<T> properties.
        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder)// where T : class, new()
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };
            var converter = new ValueConverter<T, string>
            (
                value => JsonSerializer.Serialize(value, options),
                value => JsonSerializer.Deserialize<T>(value, options)!// ?? new T()
            );
            var comparer = new ValueComparer<T>
            (
                (left, right) => JsonSerializer.Serialize(left, options) == JsonSerializer.Serialize(right, options),
                value => value == null ? 0 : JsonSerializer.Serialize(value, options).GetHashCode(),
                value => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value, options), options)!// ?? new T()
            );
            return propertyBuilder.HasConversion(converter, comparer);
        }

        /// <summary>
        /// Configures a property so that the property value is converted to the database as
        /// an Array and from the database as an IReadOnlyCollection.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="propertyBuilder">Property builder API for configuring a Microsoft.EntityFrameworkCore.Metadata.IMutableProperty.</param>
        /// <returns>The same builder instance so that multiple configuration calls can be chained.</returns>
        public static PropertyBuilder<IReadOnlyCollection<T>> HasArrayConversion<T>(
            this PropertyBuilder<IReadOnlyCollection<T>> propertyBuilder)
        {
            var converter = new ValueConverter<IReadOnlyCollection<T>, T[]>
            (
                list => list.ToArray(),
                array => array.ToList()
            );
            return propertyBuilder.HasConversion(converter);
        }

        /// <summary>
        /// Configures the property so that the property value is converted to and from the
        /// database using <see cref="List<string>">List<string></see>.
        /// </summary>
        /// <typeparam name="T">The target type of the value.</typeparam>
        /// <param name="propertyBuilder">Property builder API for configuring a Microsoft.EntityFrameworkCore.Metadata.IMutableProperty.</param>
        /// <returns>The same builder instance so that multiple configuration calls can be chained.</returns>
        // Note: restriction is commented out so that it can be used for IReadOnlyCollection<T> properties.
        public static PropertyBuilder<T> HasStringListConversion<T>(this PropertyBuilder<T> propertyBuilder)
            where T : IList<string>?
        {
            var converter = new ValueConverter<IList<string>?, string?>
            (
                value => value != null ? string.Join("|", value) : null,
                value => !string.IsNullOrEmpty(value) ? value.Split("|", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList() : default
            );
            var comparer = new ValueComparer<IList<string>?>
            (
                (left, right) => CheckStringListEquality(left, right),
                value => value == null ? 0 : value.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode()))
            );
            return propertyBuilder.HasConversion(converter, comparer);
        }

        [ExcludeFromCodeCoverage]
        private static bool CheckStringListEquality(IList<string>? left, IList<string>? right)
        {
            if (left != null && right != null)
            {
                return left.SequenceEqual(right);
            }
            else if (left == null && right == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Indicates that this field should be encrypted in the data store. Encryption is applied via a value converter in
        /// KC.Persistence.Common.BaseDbContext.InitEncryptionValueConverter()
        /// </summary>
        /// <param name="propertyBuilder">Property builder API for configuring a Microsoft.EntityFrameworkCore.Metadata.IMutableProperty.</param>
        /// <param name="algorithm">Algorithm to use for encryption. (Currently not implemented)</param>
        /// <returns>The same builder instance so that multiple configuration calls can be chained.</returns>
        public static PropertyBuilder<string?> HasEncryption(this PropertyBuilder<string?> propertyBuilder,
            EncryptionAlgorithm algorithm = EncryptionAlgorithm.AES256)
        {
            return propertyBuilder.HasAnnotation(CustomAnnotations.Encryption, algorithm);
        }

        /// <summary>
        /// Adds metadata about the sensitivity classification to the associated database column.
        /// </summary>
        /// <typeparam name="T">Property type mask is being added to.</typeparam>
        /// <param name="propertyBuilder"></param>
        /// <param name="label">Human readable name of the sensitivity label.
        /// Sensitivity labels represent the sensitivity of the data stored in the database column.</param>
        /// <param name="informationType">Human readable name of the information type.
        /// Information types are used to describe the type of data being stored in the database column.</param>
        /// <param name="rank">Identifier based on a predefined set of values which define sensitivity rank.
        /// Used by other services like Advanced Threat Protection to detect anomalies based on their rank.</param>
        /// <returns>The same builder instance so that multiple configuration calls can be chained.</returns>
        public static PropertyBuilder<T> HasSensitivityClassification<T>(this PropertyBuilder<T> propertyBuilder,
            string label, string informationType, SensitivityRank rank)
        {
            var classification = new SensitivityClassification(label, informationType, rank);
            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            return propertyBuilder.HasAnnotation(CustomAnnotations.SensitivityClassification,
                JsonSerializer.Serialize(classification, options));
        }

        /// <summary>
        /// Indicates that this field should be masked when selected in a query.
        /// </summary>
        /// <typeparam name="T">Property type mask is being added to.</typeparam>
        /// <param name="propertyBuilder">Property builder API for configuring a Microsoft.EntityFrameworkCore.Metadata.IMutableProperty.</param>
        /// <param name="function">Masking function to apply. Available functions: <see cref="DataMaskFunctions">DataMaskFunctions</see></param>
        /// <returns>The same builder instance so that multiple configuration calls can be chained.</returns>
        public static PropertyBuilder<T> HasDataMask<T>(this PropertyBuilder<T> propertyBuilder, string? function = "default()")
        {
            return propertyBuilder.HasAnnotation(CustomAnnotations.DynamicDataMask, function);
        }
    }
}
