namespace Strings.ResourceGenerator.Generators;

/// <summary>
/// A class that represents a parameteer to a resource string
/// </summary>
public class ResourceStringParameter
{
    /// <summary>
    /// Initializes a new instance of a parameter
    /// </summary>
    /// <param name="name">The parameter name</param>
    /// <param name="orgName">The original parameter name</param>
    /// <param name="format">The parameter format specifier</param>
    /// <param name="type">The parameter type</param>
    /// <param name="order">The parameter's order</param>
    public ResourceStringParameter(string name, string orgName, string format, string type, int order)
    {
        Name = name;
        OriginalName = orgName;
        Format = format;
        Type = type;
        Order = order;
    }

    /// <summary>
    /// The parameter name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The original parameter name
    /// </summary>
    public string OriginalName { get; }

    /// <summary>
    /// The parameter's format
    /// </summary>
    public string Format { get; }

    /// <summary>
    /// The parameter's type
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// The parameter's order (among other parameters)
    /// </summary>
    public int Order { get; }
}
