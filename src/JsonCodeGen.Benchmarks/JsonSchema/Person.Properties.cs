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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Corvus.Json;
using Corvus.Json.Internal;

namespace GenFromJsonSchema;
/// <summary>
/// A type generated from a JsonSchema specification.
/// </summary>
public readonly partial struct Person
{
    /// <summary>
    /// JSON property name for <see cref = "Name"/>.
    /// </summary>
    public static readonly ReadOnlyMemory<byte> NameUtf8JsonPropertyName = new byte[]{110, 97, 109, 101};
    /// <summary>
    /// JSON property name for <see cref = "Name"/>.
    /// </summary>
    public const string NameJsonPropertyName = "name";
    /// <summary>
    /// JSON property name for <see cref = "DateOfBirth"/>.
    /// </summary>
    public static readonly ReadOnlyMemory<byte> DateOfBirthUtf8JsonPropertyName = new byte[]{100, 97, 116, 101, 79, 102, 66, 105, 114, 116, 104};
    /// <summary>
    /// JSON property name for <see cref = "DateOfBirth"/>.
    /// </summary>
    public const string DateOfBirthJsonPropertyName = "dateOfBirth";
    /// <summary>
    /// Gets Name.
    /// </summary>
    public GenFromJsonSchema.PersonName Name
    {
        get
        {
            if ((this.backing & Backing.JsonElement) != 0)
            {
                if (this.jsonElementBacking.ValueKind != JsonValueKind.Object)
                {
                    return default;
                }

                if (this.jsonElementBacking.TryGetProperty(NameUtf8JsonPropertyName.Span, out JsonElement result))
                {
                    return new GenFromJsonSchema.PersonName(result);
                }
            }

            if ((this.backing & Backing.Object) != 0)
            {
                if (this.objectBacking.TryGetValue(NameJsonPropertyName, out JsonAny result))
                {
                    return result.As<GenFromJsonSchema.PersonName>();
                }
            }

            return default;
        }
    }

    /// <summary>
    /// Gets DateOfBirth.
    /// </summary>
    public Corvus.Json.JsonDate DateOfBirth
    {
        get
        {
            if ((this.backing & Backing.JsonElement) != 0)
            {
                if (this.jsonElementBacking.ValueKind != JsonValueKind.Object)
                {
                    return default;
                }

                if (this.jsonElementBacking.TryGetProperty(DateOfBirthUtf8JsonPropertyName.Span, out JsonElement result))
                {
                    return new Corvus.Json.JsonDate(result);
                }
            }

            if ((this.backing & Backing.Object) != 0)
            {
                if (this.objectBacking.TryGetValue(DateOfBirthJsonPropertyName, out JsonAny result))
                {
                    return result.As<Corvus.Json.JsonDate>();
                }
            }

            return default;
        }
    }

    /// <summary>
    /// Tries to get the validator for the given property.
    /// </summary>
    /// <param name = "property">The property for which to get the validator.</param>
    /// <param name = "hasJsonElementBacking"><c>True</c> if the object containing the property has a JsonElement backing.</param>
    /// <param name = "propertyValidator">The validator for the property, if provided by this schema.</param>
    /// <returns><c>True</c> if the validator was found.</returns>
    public bool __TryGetCorvusLocalPropertiesValidator(in JsonObjectProperty property, bool hasJsonElementBacking, [NotNullWhen(true)] out ObjectPropertyValidator? propertyValidator)
    {
        if (hasJsonElementBacking)
        {
            if (property.NameEquals(NameUtf8JsonPropertyName.Span))
            {
                propertyValidator = __CorvusValidateName;
                return true;
            }
            else if (property.NameEquals(DateOfBirthUtf8JsonPropertyName.Span))
            {
                propertyValidator = __CorvusValidateDateOfBirth;
                return true;
            }
        }
        else
        {
            if (property.NameEquals(NameJsonPropertyName))
            {
                propertyValidator = __CorvusValidateName;
                return true;
            }
            else if (property.NameEquals(DateOfBirthJsonPropertyName))
            {
                propertyValidator = __CorvusValidateDateOfBirth;
                return true;
            }
        }

        propertyValidator = null;
        return false;
    }

    /// <summary>
    /// Creates an instance of a <see cref = "Person"/>.
    /// </summary>
    public static Person Create(GenFromJsonSchema.PersonName name, Corvus.Json.JsonDate? dateOfBirth = null)
    {
        var builder = ImmutableDictionary.CreateBuilder<JsonPropertyName, JsonAny>();
        builder.Add(NameJsonPropertyName, name.AsAny);
        if (dateOfBirth is Corvus.Json.JsonDate dateOfBirth__)
        {
            builder.Add(DateOfBirthJsonPropertyName, dateOfBirth__.AsAny);
        }

        return builder.ToImmutable();
    }

    /// <summary>
    /// Sets name.
    /// </summary>
    /// <param name = "value">The value to set.</param>
    /// <returns>The entity with the updated property.</returns>
    public Person WithName(in GenFromJsonSchema.PersonName value)
    {
        return this.SetProperty(NameJsonPropertyName, value);
    }

    /// <summary>
    /// Sets dateOfBirth.
    /// </summary>
    /// <param name = "value">The value to set.</param>
    /// <returns>The entity with the updated property.</returns>
    public Person WithDateOfBirth(in Corvus.Json.JsonDate value)
    {
        return this.SetProperty(DateOfBirthJsonPropertyName, value);
    }

    private static ValidationContext __CorvusValidateName(in JsonObjectProperty property, in ValidationContext validationContext, ValidationLevel level)
    {
        return property.ValueAs<GenFromJsonSchema.PersonName>().Validate(validationContext, level);
    }

    private static ValidationContext __CorvusValidateDateOfBirth(in JsonObjectProperty property, in ValidationContext validationContext, ValidationLevel level)
    {
        return property.ValueAs<Corvus.Json.JsonDate>().Validate(validationContext, level);
    }
}