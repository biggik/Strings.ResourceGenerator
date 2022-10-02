# Strings.ResourceGenerator
Roslyn source generator that takes strings from various file formats and creates resource accessors that access strings resources in a controlled manner

Current support is for:

* .strings files (one per language, e.g. Errors.strings and Errors.de.strings)
* .json files (multiple language support)
* .yaml files (multiple language support)

The generated accessors use the current region when selecting the language to pick strings from at runtime.

For all of the formats the following applies:
* Keys must be unique for each string resource
* Values can either use standard formatting (e.g. {0}, {1}, etc) or interpolation (e.g. {name}), but not both
* Signatures must match for all languages

Parameters to strings also support type specifiers, formatting specifiers and signature ordering
* Format: `:format`, e.g. `"String with {0:n2} formatted"`
* Type: `@type`, e.g. `"String with {name@string}"`
* Order: `@order`, e.g. `String with {name@string@1}"` (order requires type as well)

Example of using all: `"String with {amount:n2@decimal@3}"'

See examples of files online on the [project site](https://github.com/biggik/Strings.ResourceGenerator/tree/main/Strings.ResourceGenerator.Examples/Resources)

## .strings files

A .strings file is simply a UTF-8 encoded flat file of string resources in a key=value format.

## .json files

.json files can be used to add strings. Json files need to be serializable from `Strings.ResourceGenerator.Models.StringsModel` (using [NewtonSoft Json](https://www.newtonsoft.com/json))

## .yaml files

.yaml files can be used to add strings. Yaml files need to be serializable from `Strings.ResourceGenerator.Models.StringsModel` (using [YamlDotNet](https://github.com/aaubry/YamlDotNet))

# How to get started

Reference the Strings.ResourceGenerate Nuget package

Modify the reference as follows
```
		<PackageReference Include="Strings.ResourceGenerator" Version="0.5.0">
			<PrivateAssets>all</PrivateAssets>
			<IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
		</PackageReference>
```

Include configuration for AdditionalFiles for the generator
```
	<ItemGroup>
		<AdditionalFiles Include="Resources\*.strings" />
		<AdditionalFiles Include="Resources\JsonExample.json" />
		<AdditionalFiles Include="Resources\NeutralExample.yaml" />
		<AdditionalFiles Include="Resources\JsonExample.json" />
		<AdditionalFiles Include="Resources\strings.config" />
	</ItemGroup>
```

## Configuration
While no configuration is required, the following are options to configure basic settings of the generator.
Configuration is done by one or more strings.config files in the project

The generic strings.config is the default where a more specific Errors.strings.config would be used for Errors.strings

The defaults for the parameters are as follows
public=false
preferConst=false
prefix=
namespace=Generated.ResourcesResources

### public

if set to true, then the string accessor clases are generated as public classes, suitable in library implementations consumed by other assemblies

### preferConst

if set to true, then, where possible, accessors are generated as `public const string` instead of `public static string`
This is not possible for multiple languages, since there a lookup is done based on the locale, so the value is never constant

### prefix

If set, then generated classes will receive the prefix as a prefix, e.g. for Errors.strings and a prefix of "Application", the generated class would be ApplicationErrors

## namespace

If set, then generated classes will be generated in the specified namespace

## Examples

See Strings.ResourceGenerator.Examples project. In its Resources folder are examples of both .strings files and a strings.config file

## Generation

Empty and commented out lines (prefixed by # or //) are ignored in string generation

## Validation

For multi-language string resources, validation is done on:
* resource keys - each language must have all the same keys
* resource string parameters - each parameterized resource key must have the same signature for all languages

## TODO

Consider supporting XLiff [https://en.wikipedia.org/wiki/XLIFF]

# Release notes

## 0.50
Initial release with support for .strings files

## 0.51
Adding support for .json and .yaml files as well as generation that supports direct access to locale resources via generated resource accessors