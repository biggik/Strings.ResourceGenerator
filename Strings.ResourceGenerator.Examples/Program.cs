using Strings.ResourceGenerator.Examples.Resources;
using System.Globalization;

namespace Strings.ResourceGenerator.Examples
{
    internal class Program
    {
        static void Main(string[] args)
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

            Console.WriteLine(JsonExample.IS.OneStringWithDouble);
            Console.WriteLine(JsonExample.Neutral.OneStringWithDouble);

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
        }
    }
}