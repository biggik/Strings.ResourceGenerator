using Strings.ResourceGenerator.Examples.Resources;

namespace Strings.ResourceGenerator.Examples;

internal class Program
{
    private static void Main(string[] args)
    {
        // Uncomment the following line to change the default culture to IS
        // CultureInfo.CurrentUICulture = new CultureInfo("is-IS");
        Console.WriteLine(new string('-', 60));
        Console.WriteLine(NeutralExample.SimplyToDemonstrateNeutralGeneration);
        Console.WriteLine(NeutralExample.OneStringWithDouble);
        Console.WriteLine(NeutralExample.OneWithInterpolation("something"));

        Console.WriteLine(new string('-', 60));
        Console.WriteLine(JsonExample.SimplyToDemonstrateNeutralGeneration);
        Console.WriteLine(JsonExample.OneStringWithDouble);
        Console.WriteLine(JsonExample.OneWithInterpolation("something"));

        Console.WriteLine(JsonExample.OneStringWithDouble);
        Console.WriteLine(JsonExample.IS.OneStringWithDouble);
        Console.WriteLine(JsonExample.Neutral.OneStringWithDouble);

        Console.WriteLine(new string('-', 60));
        Console.WriteLine(MultiLocaleStrings.SimpleString);
        Console.WriteLine(MultiLocaleStrings.MixOfExcapedAndUnescaped("value"));

        Console.WriteLine(new string('-', 60));
        Console.WriteLine(MultiLanguageExample.InterpolatedFormatString("interpolation"));
        Console.WriteLine(MultiLanguageExample.InterpolatedWithFormatting(2));
        Console.WriteLine(MultiLanguageExample.InterpolatedWithOrderingTypeAndFormatting("description", 1));
        Console.WriteLine(MultiLanguageExample.InterpolatedWithTypeAndFormatting(DateTime.Now));
        Console.WriteLine(MultiLanguageExample.SimpleString);
        Console.WriteLine(MultiLanguageExample.StandardFormatString("std"));
        Console.WriteLine(MultiLanguageExample.StandardWithFormatting(2));
        Console.WriteLine(MultiLanguageExample.StandardWithOrderingTypeAndFormatting("description", 1));
        Console.WriteLine(MultiLanguageExample.StandardWithTypeAndFormatting(DateTime.Now));

        // Also accessible via locale accessors
        Console.WriteLine(MultiLanguageExample.IS.SimpleString);
        Console.WriteLine(MultiLanguageExample.Neutral.SimpleString);

        // Messages
        Console.WriteLine(Messages.SimpleString);
        Console.WriteLine(Messages.MultiLineRaw);
        Console.WriteLine(Messages.SimpleInterpolationString("interpolated"));
    }
}
