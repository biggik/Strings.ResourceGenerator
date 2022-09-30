using Strings.ResourceGenerator.Generators.YamlFile;
using Strings.ResourceGenerator.Models;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using YamlDotNet.Serialization;

namespace UnitTests
{
    public class YamlComplerTests
    {
        [Fact]
        public void MultiLocaleResourceTest()
        {
            var yaml = @"config:
  namespace: some.namespace
  prefix: ah 
  public: false
  prefer_const: true
strings:
- key: OneStringWithDouble
  value: '{{Url}} parameter'
  context: Context for id 1
- key: SimplyToDemonstrateNeutralGeneration
  value: This is only here for that purpose
  context: Context for id 1
";

            var generator = YamlProvider.Provide("myfile.yaml", "MyClass", yaml);
            var src = generator.Generate();
            File.WriteAllText(@"c:\tmp\generated.cs", src);
            Assert.NotNull(src);
        }

        [Fact]
        public void Roundtrip_SerializeToYaml()
        {
            var model = new StringsModel
            {
                Config = new StringConfiguration
                {
                    Prefix = null,
                    GeneratePublic = true,
                    NameSpace = "Some.Namespace",
                    PreferConstOverStatic = false
                },
                Strings = new List<ResourceStringModel>
                {
                    new ResourceStringModel
                    {
                        Key = "FirstKey",
                        Value = "FirstValue {{URL}}",
                        Context = "Context for first value"
                    },
                    new ResourceStringModel
                    {
                        Key = "SecondKey",
                        Value = "{{Url}} SecondValue"
                    }
                }
            };

            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(model);
            File.WriteAllText(@"c:\tmp\model.yaml", yaml);

            var deserializer = new DeserializerBuilder()
                //.WithNamingConvention(CamelCaseNamingConvention.Instance)
                //.WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml
                .Build();
            var model2 = deserializer.Deserialize<StringsModel>(yaml);

            Assert.True(model.Strings.Count == model2.Strings.Count);
        }
    }
}