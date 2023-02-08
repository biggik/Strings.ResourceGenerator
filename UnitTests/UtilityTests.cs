using Strings.ResourceGenerator.Models;
using Strings.ResourceGenerator.Helpers;
using Strings.ResourceGenerator.Generators.StringsFile;
using FluentAssertions;
using System.Web;

namespace UnitTests
{
    public class UtilityTests
    {
        [Theory]
        [InlineData("abcdef", "abcdef")]
        [InlineData("// abcdef", "// abcdef")]
        [InlineData("// <abcdef>", "// &lt;abcdef&gt;")]
        [InlineData("The password length must be between 10 and 128 characters(inclusive) and contain at least 1 uppercase, 1 lowercase, 1 digit and 1 special character from the following set !@#€₽¢$₩¥£&*%<>()+°¨~=aaa|?:;,.[]^/{}_-.",
                    "The password length must be between 10 and 128 characters(inclusive) and contain at least 1 uppercase, 1 lowercase, 1 digit and 1 special character from the following set !@#€₽¢$₩¥£&amp;*%&lt;&gt;()+°¨~=aaa|?:;,.[]^/{}_-.")]

        public void DocumentationEncoder_Converts_Correctly(string unencoded, string expected)
        {
            DocumentationEncoder.Encode(unencoded).Should().Be(expected);
        }
    }
}