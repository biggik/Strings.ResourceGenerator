using Shouldly;
using Strings.ResourceGenerator.Exceptions;

namespace UnitTests;

public class StringGeneratorExceptionTests
{
    [Fact]
    public void Constructor_PreservesMessageAndMaterializesErrors()
    {
        var errors = new[] { "e1", "e2" };
        var ex = new StringGeneratorException("Generation failed", errors);

        ex.Message.ShouldBe("Generation failed");
        ex.Errors.ShouldBe(errors);
        ex.Errors.ShouldNotBeSameAs(errors); // ToList() copy
    }
}
