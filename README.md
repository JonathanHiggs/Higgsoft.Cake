# Higgsoft.Cake

Cake build extensions to make builds great again


## Getting Started

Add the package to your cake build script:

```csharp
#addin nuget:?package=Higgsoft.Cake
```

Then use the extension methods:

```csharp
Task("check")
    .Does(() => Check(settings => { settings.StagedChanges = false }));
```