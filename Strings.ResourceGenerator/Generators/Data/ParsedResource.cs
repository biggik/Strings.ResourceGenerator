using System.Collections.Generic;
using Strings.ResourceGenerator.Generators.Parsers;
using Strings.ResourceGenerator.Helpers;
using Strings.ResourceGenerator.Models;

namespace Strings.ResourceGenerator.Generators.Data
{
    /// <summary>
    /// A class that represents a parsed resource string
    /// </summary>
    internal class ParsedResource
    {
        private readonly string property;
        private readonly string staticGetter;
        private readonly string getter;
        private readonly string interfaceDefinition;
        private readonly ResourceString resourceString;

        /// <summary>
        /// A representation of this resource as a static property
        /// </summary>
        public string StaticProperty
        {
            get
            {
                var decl = property.Replace("<modifier>", "public static ");

                return Splitter.SplitDeclAndImpl(decl, staticGetter, "               ");
                // TODO: what is the difference to PublicStaticProperty?
            }
        }

        /// <summary>
        /// A representation of this resource as an interface declaration
        /// </summary>
        public string InterfaceDeclaration => property.Replace("<modifier>", "") + interfaceDefinition;

        /// <summary>
        /// A representation of this resource as a public property
        /// </summary>
        public string PublicProperty => property.Replace("<modifier>", "public ");

        /// <summary>
        /// A representation of this resource as a public static property
        /// </summary>
        public string PublicStaticProperty(bool preferConstOverStatic)
            => property.Replace("<modifier>",
                preferConstOverStatic ? "public const " : "public static ");

        /// <summary>
        /// A representation of this resource as a class line
        /// </summary>
        public string ClassLine(bool preferConstOverStatic) 
            => getter.Replace("<string>", resourceString.CleanValueImplementation)
                     .Replace("<getter_operator>", preferConstOverStatic ? "=" : "=>");

        /// <summary>
        /// Constructs a new instance of a resource string
        /// </summary>
        /// <param name="parser">The parser for the resource</param>
        /// <param name="resourceString">The resource string to parse</param>
        public ParsedResource(ResourceParser parser, ResourceString resourceString)
        {
            this.resourceString = resourceString;

            if (parser.StringType != StringType.Simple)
            {
                property = $"        <modifier>string {resourceString.Key}({string.Join(", ", parser.SignatureParametersWithType)})";

                if (parser.StringType == StringType.Format)
                {
                    getter = $"           <getter_operator> Format(\"<string>\", {string.Join(", ", parser.SignatureParameters)});";
                }
                else if (parser.StringType == StringType.Interpolation)
                {
                    getter = $"           <getter_operator> $\"<string>\";";
                }
                else
                {
                    getter = $"           <getter_operator> \"<string>\";";
                }
                staticGetter = $" => Current.{resourceString.Key}({string.Join(", ", parser.SignatureParameters)});";
                interfaceDefinition = ";";
            }
            else
            {
                property = $"        <modifier>string {resourceString.Key}";
                getter = "           <getter_operator> \"<string>\";";
                staticGetter = $" => Current.{resourceString.Key};";
                interfaceDefinition = " { get; }";
            }
        }
    }
}
