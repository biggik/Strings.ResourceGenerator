using Shouldly;
using Strings.ResourceGenerator.Helpers;

namespace UnitTests.Helpers;

public class FixedStringLookupTests
{
    // lookupTable[length - 1]: bucket for strings of that length (see FixedStringLookup.Contains).
    private static readonly string[][] SampleTable =
    [
        null!, // length 1 — unused
        new[] { "aa", "bb", "zz" }, // length 2 at index 1 — sorted per lookup expectations
    ];

    [Fact]
    public void Contains_ReturnsFalse_ForEmptyString()
    {
        FixedStringLookup.Contains(SampleTable, "", false).ShouldBeFalse();
    }

    [Fact]
    public void Contains_ReturnsFalse_WhenLengthBucketMissing()
    {
        // length 3 requires index 2; table has only indices 0–1
        FixedStringLookup.Contains(SampleTable, "aaa", false).ShouldBeFalse();
    }

    [Theory]
    [InlineData("aa", true)]
    [InlineData("bb", true)]
    [InlineData("ab", false)]
    [InlineData("ZY", false)]
    public void Contains_HonorsExactMatch(string value, bool expected)
    {
        FixedStringLookup.Contains(SampleTable, value, ignoreCase: false).ShouldBe(expected);
    }

    [Fact]
    public void Contains_IgnoreCase_FindsLowercaseBucketEntry()
    {
        FixedStringLookup.Contains(SampleTable, "AA", ignoreCase: true).ShouldBeTrue();
    }
}
