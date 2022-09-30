# Flatfile.ResourceGenerator
Roslyn source generator that takes flatfile strings and creates resource accessors that access strings resources in a controlled manner

Multiple languages are supported by incorporating the region in the file name as &lt;basename&gt;.&lt;region&gt;.strings, e.g.

* errors.strings [Neutral language]
* errors.de.strings [German language]

## .strings files

A .strings file is simply a UTF-8 encoded flat file of string resources in a key=value format.
Key names must be unique within a file, and both standard (e.g. {0}, {1}, etc) parameters and named (e.g. interpolated such as {key}, {value}, etc) parameters are supported.

Parameters also support the following specifications:
* format specifier of the form <code>:format</code>, e.g. {key:n2}
* type specifier of the form <code>@type</code>, e.g. {key:n2@int}
* order specifier of the form <code>@order</code>, e.g. {key:n2@int@0}

And thus different .strings files for the same resource can use different formatting and different order of parameters

## How to get started

Reference the FlatFile.ResourceGenerate Nuget package

```
	<ItemGroup>
		<AdditionalFiles Include="Resources\*.strings" />
		<AdditionalFiles Include="Resources\strings.config" />
	</ItemGroup>
```

## Configuration
While no configuration is required, the following are options to configure basic settings of the generator.
Configuration is done by one or more strings.config files in the project

The generic strings.config is the default where a more specific errors.strings.config would be used for errors.strings

The defaults for the parameters are as follows
public=false
prefix=
namespace=Generated.ResourcesResources

## Examples

See FlatFile.ResourceGenerator.Examples project. In its Resources folder are examples of both .strings files and a strings.config file

## Generation

Empty and commented out lines (prefixed by # or //) are ignored in string generation

## Validation

For multi-language string resources, validation is done on:
* resource keys - each language must have all the same keys
* resource string parameters - each parameterized resource key must have the same signature for all languages