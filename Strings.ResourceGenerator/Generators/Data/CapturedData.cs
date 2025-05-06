namespace Strings.ResourceGenerator.Generators.Data;

internal class CapturedData
{
    public CapturedData(string original, string type, string typeKey, string format, string prefixedName, string name, int order)
    {
        Original = original;
        Type = type;
        TypeKey = typeKey;
        Format = format;
        PrefixedName = prefixedName;
        Name = name;
        Order = order;
    }

    public string Original { get; }
    public string Type { get; }
    public string TypeKey { get; }
    public string Format { get; }
    public string PrefixedName { get; }
    public string Name { get; }
    public int Order { get; }

    public override string ToString()
    {
        return $"Original: {Original}, Type: {Type}, TypeKey: {TypeKey}, Format: {Format}, PrefixedName: {PrefixedName}, Name: {Name}, Order: {Order}";
    }
}