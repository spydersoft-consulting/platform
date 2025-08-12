# Options Configuration

This library provides extensions (`AddSpydersoftOptions`) and attributes to standardize options configuration in ASP.NET Core. The system allows you to register options classes by decorating them with a custom attribute and then calling a single extension method in your startup code.

## Decorating Classes for Options


## SpydersoftOptionsAttribute

Decorate your options classes with `SpydersoftOptionsAttribute` to enable automatic registration. The attribute takes a required `sectionName` and an optional comma-delimited `tags` string. Tags allow you to group or filter which options are registered at startup. **If you do not specify any tags, the options class will always be registered regardless of the tags passed to `AddSpydersoftOptions`.**

### Example


```csharp
[SpydersoftOptions("MyOptionSection", "root")]
public class MyOptionSection
{
  public string Option1 { get; set; } = "Option1";
  public string Option2 { get; set; } = "Option2";
}

[SpydersoftOptions("NestedOptionSection", "nested")]
public class NestedOptionSection
{
  public string NestedOption1 { get; set; } = "NestedOption1";
  public string NestedOption2 { get; set; } = "NestedOption2";
}
```

builder.AddSpydersoftOptions(["root"]);
builder.AddSpydersoftOptions(["nested"], "MySection");

## Registering Options in Program.cs


Call `AddSpydersoftOptions` on your `WebApplicationBuilder` to register all decorated options classes. You must specify which tags to include. Optionally, you can provide a `sectionPrefix` to nest configuration sections.

**Registration logic:**
- If an options class has no tags, it is always registered.
- If an options class has tags, it is only registered if any of its tags match the provided list.

```csharp
builder.AddSpydersoftOptions(["root"]); // Registers all options with the "root" tag
builder.AddSpydersoftOptions(["nested"], "MySection"); // Registers all options with the "nested" tag, using "MySection" as a prefix
```


The extension will scan all loaded assemblies for classes decorated with `SpydersoftOptionsAttribute`. For each class:
- If it has no tags, it will be registered unconditionally.
- If it has tags, it will be registered only if any of its tags match the provided list.
Registration is performed using `IServiceCollection.Configure<TOptions>()` with the appropriate configuration section.

### Section Prefix

If you provide a `sectionPrefix`, the configuration section will be nested. For example, with `sectionPrefix = "MySection"` and `SectionName = "NestedOptionSection"`, the configuration path will be `MySection:NestedOptionSection`.

### Example appsettings.json

```json
{
  "MyOptionSection": {
    "Option1": "Value1FromOptions",
    "Option2": "Value2FromOptions"
  },
  "MySection": {
    "NestedOptionSection": {
      "NestedOption1": "NestedValue1FromOptions",
      "NestedOption2": "NestedValue2FromOptions"
    }
  }
}
```

## Notes

- The `tags` parameter in the attribute is a comma-separated string. The extension will register a class if **any** of its tags match the provided list.
- If an options class has no tags, it will always be registered, regardless of the tags you pass to `AddSpydersoftOptions`.
- If no matching tag is found for a tagged options class, it will be skipped.
- The extension uses reflection to find and register all eligible options classes at startup.
