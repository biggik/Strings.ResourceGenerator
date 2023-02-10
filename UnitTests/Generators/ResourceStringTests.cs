using FluentAssertions;
using Strings.ResourceGenerator.Generators;
using Strings.ResourceGenerator.Models;

namespace UnitTests.Generators
{
    public class ResourceStringTests
    {
        [Fact]
        public void ResourceString_Simple_Correctly()
        {
            var undertest = new ResourceString(null, "MyKey", "My simple value");

            // Simple assertions
            undertest.Locale.Should().Be(null);
            undertest.Key.Should().Be("MyKey");
            undertest.Value.Should().Be("My simple value");

            undertest.StringType.Should().Be(StringType.Simple);
            undertest.CleanValueImplementation.Should().Be("My simple value");
            undertest.Parameters.Count().Should().Be(0);

            //rs.StaticProperty.Should().Be();
            //rs.InterfaceDeclaration.Should().Be();
            //rs.PublicProperty.Should().Be();
            //rs.PublicStaticProperty.Should().Be();
            //rs.ClassLine.Should().Be();
        }

        [Theory]
        [InlineData("My simple value: {0}", "My simple value: {0}", "", "object")]
        [InlineData("My simple value: {0@string}", "My simple value: {0}", "", "string")]
        [InlineData("My simple value: {0:n2@int}", "My simple value: {0:n2}", ":n2", "int")]
        public void ResourceString_StandardParameter_Correctly(string value, string expectedValue, string expectedFormat, string expectedType)
        {
            var undertest = new ResourceString(null, "MyKey", value);

            // Simple assertions
            undertest.Locale.Should().Be(null);
            undertest.Key.Should().Be("MyKey");
            undertest.Value.Should().Be(expectedValue);

            undertest.StringType.Should().Be(StringType.Format);
            undertest.CleanValueImplementation.Should().Be(expectedValue);

            undertest.Parameters.Count().Should().Be(1);
            undertest.Parameters[0].Name.Should().Be("arg0");
            undertest.Parameters[0].OriginalName.Should().Be("0");
            undertest.Parameters[0].Format.Should().Be(expectedFormat);
            undertest.Parameters[0].Type.Should().Be(expectedType);
            undertest.Parameters[0].Order.Should().Be(0);
        }

        [Fact]
        public void ResourceString_StandardTwoParameter_Correctly()
        {
            string value = "My {1:n2} simple value: {0@string}";
            string expectedValue = "My {1:n2} simple value: {0}";
            var undertest = new ResourceString(null, "MyKey", value);

            // Simple assertions
            undertest.Locale.Should().Be(null);
            undertest.Key.Should().Be("MyKey");
            undertest.Value.Should().Be(expectedValue);

            undertest.StringType.Should().Be(StringType.Format);
            undertest.CleanValueImplementation.Should().Be(expectedValue);

            undertest.Parameters.Count().Should().Be(2);

            undertest.Parameters[0].Name.Should().Be("arg0");
            undertest.Parameters[0].OriginalName.Should().Be("0");
            undertest.Parameters[0].Format.Should().Be("");
            undertest.Parameters[0].Type.Should().Be("string");
            undertest.Parameters[0].Order.Should().Be(0);

            undertest.Parameters[1].Name.Should().Be("arg1");
            undertest.Parameters[1].OriginalName.Should().Be("1");
            undertest.Parameters[1].Format.Should().Be(":n2");
            undertest.Parameters[1].Type.Should().Be("object");
            undertest.Parameters[1].Order.Should().Be(1);
        }

        [Theory]
        [InlineData("{0}", "arg0", 0, StringType.Format)]
        [InlineData("{value}", "value", 100, StringType.Interpolation)]
        public void ResourceString_EscapedAndUnescaped_Correctly(string parameter, string parameterName, int expectedOrder, StringType expectedStringType)
        {
            string value = "{{ParameterName}} should have value " + parameter;
            string expectedValue = value;
            var undertest = new ResourceString(null, "MyKey", value);

            // Simple assertions
            undertest.Locale.Should().Be(null);
            undertest.Key.Should().Be("MyKey");
            undertest.Value.Should().Be(expectedValue);

            undertest.StringType.Should().Be(expectedStringType);
            undertest.CleanValueImplementation.Should().Be(expectedValue);

            undertest.Parameters.Count().Should().Be(1);

            undertest.Parameters[0].Name.Should().Be(parameterName);
            undertest.Parameters[0].OriginalName.Should().Be(parameter.Trim('{','}'));
            undertest.Parameters[0].Format.Should().Be("");
            undertest.Parameters[0].Type.Should().Be("object");
            undertest.Parameters[0].Order.Should().Be(expectedOrder);
        }
    }
}
