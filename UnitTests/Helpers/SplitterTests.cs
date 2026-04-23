using Shouldly;
using Strings.ResourceGenerator.Helpers;

namespace UnitTests.Helpers;

public class SplitterTests
{
    [Fact]
    public void SplitDeclAndImpl_JoinsDeclarationAndTrimmedImplementation()
    {
        Splitter.SplitDeclAndImpl("public static string Foo", "   => Current.Bar();  ")
            .ShouldBe("public static string Foo => Current.Bar();");
    }
}
