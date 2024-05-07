//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using Corvus.Json;

namespace GenFromJsonSchema;
/// <summary>
/// Generated from JSON Schema.
/// </summary>
public readonly partial struct OtherNames
{
    /// <summary>
    /// Gets the value as a <see cref = "GenFromJsonSchema.PersonNameElement"/>.
    /// </summary>
    public GenFromJsonSchema.PersonNameElement AsPersonNameElement
    {
        get
        {
            return (GenFromJsonSchema.PersonNameElement)this;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this is a valid <see cref = "GenFromJsonSchema.PersonNameElement"/>.
    /// </summary>
    public bool IsPersonNameElement
    {
        get
        {
            return ((GenFromJsonSchema.PersonNameElement)this).IsValid();
        }
    }

    /// <summary>
    /// Gets the value as a <see cref = "GenFromJsonSchema.PersonNameElement"/>.
    /// </summary>
    /// <param name = "result">The result of the conversion.</param>
    /// <returns><c>True</c> if the conversion was valid.</returns>
    public bool TryGetAsPersonNameElement(out GenFromJsonSchema.PersonNameElement result)
    {
        result = (GenFromJsonSchema.PersonNameElement)this;
        return result.IsValid();
    }

    /// <summary>
    /// Gets the value as a <see cref = "GenFromJsonSchema.PersonNameElementArray"/>.
    /// </summary>
    public GenFromJsonSchema.PersonNameElementArray AsPersonNameElementArray
    {
        get
        {
            return (GenFromJsonSchema.PersonNameElementArray)this;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this is a valid <see cref = "GenFromJsonSchema.PersonNameElementArray"/>.
    /// </summary>
    public bool IsPersonNameElementArray
    {
        get
        {
            return ((GenFromJsonSchema.PersonNameElementArray)this).IsValid();
        }
    }

    /// <summary>
    /// Gets the value as a <see cref = "GenFromJsonSchema.PersonNameElementArray"/>.
    /// </summary>
    /// <param name = "result">The result of the conversion.</param>
    /// <returns><c>True</c> if the conversion was valid.</returns>
    public bool TryGetAsPersonNameElementArray(out GenFromJsonSchema.PersonNameElementArray result)
    {
        result = (GenFromJsonSchema.PersonNameElementArray)this;
        return result.IsValid();
    }
}