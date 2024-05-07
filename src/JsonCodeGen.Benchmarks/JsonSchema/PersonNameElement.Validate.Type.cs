//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System.Text.Json;
using Corvus.Json;

namespace GenFromJsonSchema;
/// <summary>
/// A type generated from a JsonSchema specification.
/// </summary>
public readonly partial struct PersonNameElement
{
    private ValidationContext ValidateType(JsonValueKind valueKind, in ValidationContext validationContext, ValidationLevel level)
    {
        ValidationContext result = validationContext;
        bool isValid = false;
        ValidationContext localResultString = Corvus.Json.Validate.TypeString(valueKind, result.CreateChildContext(), level);
        if (level == ValidationLevel.Flag && localResultString.IsValid)
        {
            return validationContext;
        }

        if (localResultString.IsValid)
        {
            isValid = true;
        }

        result = result.MergeResults(isValid, level, localResultString);
        return result;
    }
}