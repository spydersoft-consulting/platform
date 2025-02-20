# Options Configuration

This library provides extensions (`AddSpydersoftOptions`) and attributes to standardize options configuration in ASP.NET Core.

## Decorating Classes for Options

A class can be decorated with `Spydersoft.Platform.Attributes.SpydersoftOptionsAttribute` to enable easy addition via the `IServiceProvider.Configure` extension.

### Example

First, create your options classes and decorate them with `Spydersoft.Platform.Attributes.SpydersoftOptionsAttribute`. The `SectionName` indicates the section to be retrieved from configuration. `Tags` is a comma-delimited list of tags for this option.

```csharp
 [SpydersoftOptions(nameof(MyOptionSection), "root")]
 public class MyOptionSection
 {
     public string Option1 { get; set; } = "Option1";

     public string Option2 { get; set; } = "Option2";
}

[SpydersoftOptions(nameof(NestedOptionSection), "nested")]
public class NestedOptionSection
{
    public string NestedOption1 { get; set; } = "NestedOption1";

    public string NestedOption2 { get; set; } = "NestedOption2";
}
```

Then, in `Program.cs`, add the options:

```csharp

builder.AddSpydersoftOptions(["root"]);
builder.AddSpydersoftOptions(["nested"], "MySection");

```

The extension will locate all classes decorated with the options attribute, and add them via the `IServiceProvider.Configure<OptionClass>()` generic.

Notice the section prefix: in the above example, `MyOptionSection` will load from the root of the configuration, looking for `MyOptionSection` in the root of the configuration. `NestedOptionSection` will load from `MySection:NestedOptionSection`.

The appsettings.json file for the above example:

```json
  "MyOptionSection": {
    "Option1": "Value1FromOptions",
    "Option2": "Value2FromOptions"
  },
  "MySection": {
    "NestedOptionSection" : {
      "NestedOption1": "NestedValue1FromOptions",
      "NestedOption2": "NestedValue2FromOptions"
    }
  }
```
