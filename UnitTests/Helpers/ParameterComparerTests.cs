using Shouldly;
using Strings.ResourceGenerator.Generators.Data;
using Strings.ResourceGenerator.Helpers;

namespace UnitTests.Helpers;

public class ParameterComparerTests
{
    [Fact]
    public void Equals_UsesPrefixedNameOnly()
    {
        var c = new ParameterComparer();
        var a = new CapturedData("{0}", "object", "Key.arg0", "", "arg0", "0", 0);
        var samePrefix = new CapturedData("{0:x}", "int", "other", "", "arg0", "0", 99);

        c.Equals(a, samePrefix).ShouldBeTrue();
        c.GetHashCode(a).ShouldBe(c.GetHashCode(samePrefix));
    }

    [Fact]
    public void Equals_FalseWhenPrefixedNamesDiffer()
    {
        var c = new ParameterComparer();
        var a = new CapturedData("{0}", "object", "Key.arg0", "", "arg0", "0", 0);
        var b = new CapturedData("{1}", "object", "Key.arg1", "", "arg1", "1", 1);

        c.Equals(a, b).ShouldBeFalse();
    }
}
