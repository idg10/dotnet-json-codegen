//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Corvus.Json;
using Corvus.Json.Internal;

namespace GenFromJsonSchema;
/// <summary>
/// A type generated from a JsonSchema specification.
/// </summary>
public readonly partial struct PersonName
{
    private static readonly ImmutableDictionary<JsonPropertyName, PropertyValidator<PersonName>> __CorvusLocalProperties = CreateLocalPropertyValidators();
    /// <summary>
    /// JSON property name for <see cref = "FamilyName"/>.
    /// </summary>
    public static readonly ReadOnlyMemory<byte> FamilyNameUtf8JsonPropertyName = new byte[]{102, 97, 109, 105, 108, 121, 78, 97, 109, 101};
    /// <summary>
    /// JSON property name for <see cref = "FamilyName"/>.
    /// </summary>
    public const string FamilyNameJsonPropertyName = "familyName";
    /// <summary>
    /// JSON property name for <see cref = "GivenName"/>.
    /// </summary>
    public static readonly ReadOnlyMemory<byte> GivenNameUtf8JsonPropertyName = new byte[]{103, 105, 118, 101, 110, 78, 97, 109, 101};
    /// <summary>
    /// JSON property name for <see cref = "GivenName"/>.
    /// </summary>
    public const string GivenNameJsonPropertyName = "givenName";
    /// <summary>
    /// JSON property name for <see cref = "OtherNames"/>.
    /// </summary>
    public static readonly ReadOnlyMemory<byte> OtherNamesUtf8JsonPropertyName = new byte[]{111, 116, 104, 101, 114, 78, 97, 109, 101, 115};
    /// <summary>
    /// JSON property name for <see cref = "OtherNames"/>.
    /// </summary>
    public const string OtherNamesJsonPropertyName = "otherNames";
    /// <summary>
    /// Gets FamilyName.
    /// </summary>
    public GenFromJsonSchema.PersonNameElement FamilyName
    {
        get
        {
            if ((this.backing & Backing.JsonElement) != 0)
            {
                if (this.jsonElementBacking.ValueKind != JsonValueKind.Object)
                {
                    return default;
                }

                if (this.jsonElementBacking.TryGetProperty(FamilyNameUtf8JsonPropertyName.Span, out JsonElement result))
                {
                    return new GenFromJsonSchema.PersonNameElement(result);
                }
            }

            if ((this.backing & Backing.Object) != 0)
            {
                if (this.objectBacking.TryGetValue(FamilyNameJsonPropertyName, out JsonAny result))
                {
                    return result.As<GenFromJsonSchema.PersonNameElement>();
                }
            }

            return default;
        }
    }

    /// <summary>
    /// Gets GivenName.
    /// </summary>
    public GenFromJsonSchema.PersonNameElement GivenName
    {
        get
        {
            if ((this.backing & Backing.JsonElement) != 0)
            {
                if (this.jsonElementBacking.ValueKind != JsonValueKind.Object)
                {
                    return default;
                }

                if (this.jsonElementBacking.TryGetProperty(GivenNameUtf8JsonPropertyName.Span, out JsonElement result))
                {
                    return new GenFromJsonSchema.PersonNameElement(result);
                }
            }

            if ((this.backing & Backing.Object) != 0)
            {
                if (this.objectBacking.TryGetValue(GivenNameJsonPropertyName, out JsonAny result))
                {
                    return result.As<GenFromJsonSchema.PersonNameElement>();
                }
            }

            return default;
        }
    }

    /// <summary>
    /// Gets OtherNames.
    /// </summary>
    public GenFromJsonSchema.OtherNames OtherNames
    {
        get
        {
            if ((this.backing & Backing.JsonElement) != 0)
            {
                if (this.jsonElementBacking.ValueKind != JsonValueKind.Object)
                {
                    return default;
                }

                if (this.jsonElementBacking.TryGetProperty(OtherNamesUtf8JsonPropertyName.Span, out JsonElement result))
                {
                    return new GenFromJsonSchema.OtherNames(result);
                }
            }

            if ((this.backing & Backing.Object) != 0)
            {
                if (this.objectBacking.TryGetValue(OtherNamesJsonPropertyName, out JsonAny result))
                {
                    return result.As<GenFromJsonSchema.OtherNames>();
                }
            }

            return default;
        }
    }

    /// <summary>
    /// Creates an instance of a <see cref = "PersonName"/>.
    /// </summary>
    public static PersonName Create(GenFromJsonSchema.PersonNameElement familyName, GenFromJsonSchema.PersonNameElement? givenName = null, GenFromJsonSchema.OtherNames? otherNames = null)
    {
        var builder = ImmutableDictionary.CreateBuilder<JsonPropertyName, JsonAny>();
        builder.Add(FamilyNameJsonPropertyName, familyName.AsAny);
        if (givenName is GenFromJsonSchema.PersonNameElement givenName__)
        {
            builder.Add(GivenNameJsonPropertyName, givenName__.AsAny);
        }

        if (otherNames is GenFromJsonSchema.OtherNames otherNames__)
        {
            builder.Add(OtherNamesJsonPropertyName, otherNames__.AsAny);
        }

        return builder.ToImmutable();
    }

    /// <summary>
    /// Sets familyName.
    /// </summary>
    /// <param name = "value">The value to set.</param>
    /// <returns>The entity with the updated property.</returns>
    public PersonName WithFamilyName(in GenFromJsonSchema.PersonNameElement value)
    {
        return this.SetProperty(FamilyNameJsonPropertyName, value);
    }

    /// <summary>
    /// Sets givenName.
    /// </summary>
    /// <param name = "value">The value to set.</param>
    /// <returns>The entity with the updated property.</returns>
    public PersonName WithGivenName(in GenFromJsonSchema.PersonNameElement value)
    {
        return this.SetProperty(GivenNameJsonPropertyName, value);
    }

    /// <summary>
    /// Sets otherNames.
    /// </summary>
    /// <param name = "value">The value to set.</param>
    /// <returns>The entity with the updated property.</returns>
    public PersonName WithOtherNames(in GenFromJsonSchema.OtherNames value)
    {
        return this.SetProperty(OtherNamesJsonPropertyName, value);
    }

    private static ImmutableDictionary<JsonPropertyName, PropertyValidator<PersonName>> CreateLocalPropertyValidators()
    {
        ImmutableDictionary<JsonPropertyName, PropertyValidator<PersonName>>.Builder builder = ImmutableDictionary.CreateBuilder<JsonPropertyName, PropertyValidator<PersonName>>();
        builder.Add(FamilyNameJsonPropertyName, __CorvusValidateFamilyName);
        builder.Add(GivenNameJsonPropertyName, __CorvusValidateGivenName);
        builder.Add(OtherNamesJsonPropertyName, __CorvusValidateOtherNames);
        return builder.ToImmutable();
    }

    private static ValidationContext __CorvusValidateFamilyName(in PersonName that, in ValidationContext validationContext, ValidationLevel level)
    {
        GenFromJsonSchema.PersonNameElement property = that.FamilyName;
        return property.Validate(validationContext, level);
    }

    private static ValidationContext __CorvusValidateGivenName(in PersonName that, in ValidationContext validationContext, ValidationLevel level)
    {
        GenFromJsonSchema.PersonNameElement property = that.GivenName;
        return property.Validate(validationContext, level);
    }

    private static ValidationContext __CorvusValidateOtherNames(in PersonName that, in ValidationContext validationContext, ValidationLevel level)
    {
        GenFromJsonSchema.OtherNames property = that.OtherNames;
        return property.Validate(validationContext, level);
    }
}