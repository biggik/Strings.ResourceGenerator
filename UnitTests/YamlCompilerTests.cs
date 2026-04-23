using Shouldly;
using Strings.ResourceGenerator.Generators.YamlFile;
using Strings.ResourceGenerator.Models;
using YamlDotNet.Serialization;

namespace UnitTests;

public class YamlCompilerTests
{
    [Fact]
    public void YamlResourceTest()
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
        File.WriteAllText($@"c:\tmp\generated.{nameof(YamlCompilerTests)}.cs", src);
        src.ShouldNotBeNull();
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
            Strings =
            [
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
            ]
        };

        var serializer = new SerializerBuilder().Build();
        var yaml = serializer.Serialize(model);
        File.WriteAllText(@"c:\tmp\model.yaml", yaml);

        var deserializer = new DeserializerBuilder()
            //.WithNamingConvention(CamelCaseNamingConvention.Instance)
            //.WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml
            .Build();
        var model2 = deserializer.Deserialize<StringsModel>(yaml);

        model2.ShouldNotBeNull();
        model2.Strings.Count.ShouldBe(model.Strings.Count);

        var byKey = model.Strings.ToDictionary(s => s.Key);
        foreach (var s in model2.Strings)
        {
            byKey[s.Key].Value.ShouldBe(s.Value);
            byKey[s.Key].Context.ShouldBe(s.Context);
        }
    }
}