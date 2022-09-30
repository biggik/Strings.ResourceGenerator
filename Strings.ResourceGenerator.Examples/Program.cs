using Strings.ResourceGenerator.Examples.Resources;

namespace Strings.ResourceGenerator.Examples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            Console.WriteLine(NeutralExample.SimplyToDemonstrateNeutralGeneration);
            Console.WriteLine(NeutralExample.OneStringWithDouble);
            Console.WriteLine(NeutralExample.OneWithInterpolation("something"));

            Console.WriteLine(JsonExample.SimplyToDemonstrateNeutralGeneration);
            Console.WriteLine(JsonExample.OneStringWithDouble);
            Console.WriteLine(JsonExample.OneWithInterpolation("something"));

            Console.WriteLine(MultiLanguageExample.InterpolatedFormatString("interpolation"));
            Console.WriteLine(MultiLanguageExample.InterpolatedWithFormatting(2));
            Console.WriteLine(MultiLanguageExample.InterpolatedWithOrderingTypeAndFormatting("description", 1));
            Console.WriteLine(MultiLanguageExample.InterpolatedWithTypeAndFormatting(DateTime.Now));
            Console.WriteLine(MultiLanguageExample.SimpleString);
            Console.WriteLine(MultiLanguageExample.StandardFormatString("std"));
            Console.WriteLine(MultiLanguageExample.StandardWithFormatting(2));
            Console.WriteLine(MultiLanguageExample.StandardWithOrderingTypeAndFormatting("description", 1));
            Console.WriteLine(MultiLanguageExample.StandardWithTypeAndFormatting(DateTime.Now));
        }
    }
}