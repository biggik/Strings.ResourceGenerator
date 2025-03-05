namespace UnitTests.Utils;

internal class DebugDump
{
    public static void Dump(string name, string s, string extension = ".cs")
    {
        var outputPath = Path.Combine(Path.GetTempPath(), "Strings.ResourceGenerator");
        Directory.CreateDirectory(outputPath);
        var file = Path.Combine(outputPath, $"generated.{name}{extension}");
        File.WriteAllText(file, s);
    }
}
