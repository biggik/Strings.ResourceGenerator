using System;
using System.Collections.Generic;
using System.IO;
using Strings.ResourceGenerator.Generators.Data;
using Strings.ResourceGenerator.Generators.Parsers;
using Strings.ResourceGenerator.Models;

namespace Strings.ResourceGenerator.Generators;

/// <summary>
/// A class that represents a resource string in the system
/// </summary>
internal class ResourceString
{
    /// <summary>
    /// The locale this resource belongs to
    /// </summary>
    public string Locale => parser.Locale;

    /// <summary>
    /// The resource key - i.e. the name of the resource string
    /// </summary>
    public string Key => parser.Key;

    /// <summary>
    /// The resource string
    /// </summary>
    public string Value => parser.Value;

    /// <summary>
    /// The string type of this resource string
    /// </summary>
    public StringType StringType => parser.StringType;

    /// <summary>
    /// Context for the string
    /// </summary>
    public string Context { get; }

    private readonly ResourceParser parser;
    private readonly ParsedResource parsed;

    /// <summary>
    /// A representation of this resource as a static property
    /// </summary>
    public string StaticProperty => parsed.StaticProperty;

    /// <summary>
    /// A representation of this resource as an interface declaration
    /// </summary>
    public string InterfaceDeclaration => parsed.InterfaceDeclaration;

    /// <summary>
    /// A representation of this resource as a public property
    /// </summary>
    public string PublicProperty => parsed.PublicProperty;

    internal List<ResourceStringParameter> Parameters => parser.Parameters;

    /// <summary>
    /// A representation of this resource as a public static property
    /// </summary>
    public string PublicStaticProperty(bool preferConstOverStatic)
        => parsed.PublicStaticProperty(preferConstOverStatic);

    /// <summary>
    /// A representation of this resource as a class line
    /// </summary>
    public string ClassLine(bool preferConstOverStatic) => parsed.ClassLine(preferConstOverStatic);

    /// <summary>
    /// The value cleaned of any unnecessary escapes
    /// </summary>
    public string CleanValueNonImplementation
        => Value.Replace("\\\\r", "\\r")
                .Replace("\\\\n", "\\n")
                .Replace("\\\\t", "\\t")
                ;

    /// <summary>
    /// The value cleaned of any unnecessary escapes
    /// </summary>
    public string CleanValueImplementation
    {
        get
        {
            var res = CleanValueNonImplementation;

            if (parser.Parameters.Count == 0)
            {
                res = res.Replace("{{", "{")
                         .Replace("}}", "}")
                         ;
            }

            return res;
        }
    }

    /// <summary>
    /// Constructs a new instance of a resource string
    /// </summary>
    /// <param name="locale">The locale this resource belongs to</param>
    /// <param name="key">The key to this resource</param>
    /// <param name="value">The raw value of the resource</param>
    /// <param name="context">Optional context</param>
    /// <exception cref="ArgumentException">In case of a bad parameter declaration, this exception is thrown</exception>
    /// <exception cref="InvalidDataException">In case of a mix of {0} and {interpolated} parameters, this exception is thrown</exception>
    public ResourceString(string locale, string key, string value, string context = null)
    {
        Context = context;
        parser = new ResourceParser(locale, key, value);
        parsed = new ParsedResource(parser, this);
    }
}
