<p align="center">
    <h3 align="center">Higgsoft.Cake</h3>
    <p align="center">Cake recipes for low-hassle builds</p>
</p>


## About

A [Cake](https://cakebuild.net/) extension that defines build recipes for applications and libraries that removes the 
boilerplate logic from build scripts

Recipes have a complete set of tasks needed for a fully automated build:
* Info - Displays project build information and settings
* Setup - Ensures all resources
* Check - Performs pre-build checks to ensure a consistent build state
* Version - Increments the version number
* ReleaseNotes - Prepares release notes
* Clean - Removes temporary files
* Build - Calls the dotnet build
* Test - Uses NUnit3 to run all unit tests
* Publish - Calls dotnet publish
* Package - Packs the results in nuget package or build artifact archive
* Commit - Commits file changes and creates a version tag
* Push - Pushes build artifacts to repos and code changes to git remote

There are optional pre and post-build steps for more complicates builds

### Built With

* [Cake](https://cakebuild.net/)
* [Cake.Git](https://github.com/cake-contrib/Cake_Git)


## Usage

The primary usage is with the recipe builds. Use the `AddDotNetLib` or `AddDotNetApp` and fill out 
the configuration before invoking the `Build.Target` and everything else is done automatically

```csharp
//////////////////////////
// Includes & Using
////////////////////////

#load nuget:?package=Higgsoft.Cake&version=0.1.0


//////////////////////////
// Definitions
////////////////////////

AddDotNetLib(lib => {
    lib.Id                          = "MyProject";
    lib.Name                        = "MyProject";
    lib.Description                 = "My project does things";
    lib.Solution                    = "MyProject";
    lib.Project                     = "MyProject";
    
    lib.SolutionDirectory           = Directory($"{Build.GitRoot}/src");
    lib.SolutionFile                = File($"{lib.SolutionDirectory}/{lib.Solution}.sln");
    lib.ProjectFile                 = File($"{lib.SolutionDirectory}/{lib.Project}/{lib.Project}.csproj");
    lib.AssemblyInfoFile            = null;
    lib.ReleaseNotesFile            = File($"{Build.GitRoot}/ReleaseNotes.md");
    lib.ReleaseNotesVNextFile       = File($"{Build.GitRoot}/ReleaseNotes.vnext.md");
    lib.PublishDirectory            = Directory($"{Build.GitRoot}/publish");
    lib.NuGetDirectory              = Directory($"{Build.GitRoot}/nuget");

    lib.SharedAssemblyInfoFile      = false;
    lib.UsePreBuildTask             = false;
    lib.UsePostBuildTask            = false;
    lib.PrepareReleaseNotes         = true;
    lib.UpdateAssemblyInfo          = false;
    lib.CommitChanges               = true;
    lib.TagVersion                  = true;

    lib.ProjectUrl                  = new Uri("https://example.com/my-project");

    lib.AddFrameworks("net48", "netstandard2.0");
    lib.AddAuthors("Jonathan Higgs");
});


//////////////////////////
// Invoke
////////////////////////

RunTarget(Build.Target);
```

### Extensions

The package has many helper methods to perform the individual actions. Reference the package from a
cake build script, then use the extension methods

```csharp
#addin nuget:?package=Higgsoft.Cake

Task("check")
    .Does(() => Check(settings => { settings.StagedChanges = false }));
```

### Running Builds

There are two types of builds, local and release. The local build will run create the 
nuget package and push it into your local nuget package repository so that it can be used with 
downstream projects

> :warning: This will rebuild the package of the same version, the build will replace the version 
in the local repository, but any downstream projects cache the package in their local `/packages` 
directory, so the package should be removed from that cache

```sh
powershell "./build.ps1" --local=True --settings_skippackageversioncheck=true
```

The release build will push the package to the specified nuget repository, eg:

```sh
powershell "./build.ps1" --local=False --nuget-repo=github --settings_skippackageversioncheck=true
```

### Release Notes & Versioning

The recipes expect two release notes files `./ReleaseNotes.md` and `./ReleaseNotes.vnext.md`. Changes
to a project should be accompanied with notes detailing the changes into the v-next file. This
file also contains a version hint in the first line that will be the version number of the package
on the next build. This feature can be turned off with the `PrepareReleaseNotes` flag


## Contributing

### Prerequisites

* [VisualStudio 2019](https://visualstudio.microsoft.com/downloads/)
* [Powershell](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell)

### Installation

For working with the source code clone the repo and open the solution file

```sh
git clone github.com:JonathanHiggs/Higgsoft.Wpf.git
```

### Building

The build uses it's own dotnet-lib recipe, so see the usage section


## License

Distributed under the MIT License. See `LICENSE` for more information.


## Contact

[Jonathan Higgs](mailto:jonathan.higgs.11@mail.wbs.ac.uk)

Project Link: [https://github.com/JonathanHiggs/Higgsoft.Cake](https://github.com/JonathanHiggs/Higgsoft.Cake)
