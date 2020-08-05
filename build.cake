//////////////////////////
// Includes & Using
////////////////////////

// Packages //
#addin nuget:?package=Cake.Git
#addin nuget:?package=Cake.FileHelpers
#addin nuget:?package=Higgsoft.Cake&version=0.0.1


//////////////////////////
// Arguments
////////////////////////

var target              = Argument("target", "Default");
var configuration       = Argument("configuration", "Release");

// Check options
var checkStaged         = Argument("check-staged", true);
var checkUncommitted    = Argument("check-uncommitted", true);
var checkUntracked      = Argument("check-untracked", true);
var requireReleaseNotes = Argument("require-release-notes", true);
var runTests            = Argument("run-tests", true);


//////////////////////////
// Files & Directories
////////////////////////

var solution            = "Higgsoft.Cake";
var project             = solution;

var root                = GitFindRootFromPath(MakeAbsolute(Directory(".")));
var solutionDir         = MakeAbsolute(Directory($"{root}/src"));
var projectDir          = MakeAbsolute(Directory($"{solutionDir}/{project}"));
var publishDir          = MakeAbsolute(Directory($"{root}/publish"));
var nugetDir            = MakeAbsolute(Directory($"{root}/nuget"));

var solutionFile        = File($"{solutionDir}/{solution}.sln");
var projectFile         = File($"{projectDir}/{project}.csproj");
var releaseNotes        = File($"{root}/ReleaseNotes.md");
var releaseNotesVNext   = File($"{root}/ReleaseNotes.vnext.md");


//////////////////////////
// Variables & Config
////////////////////////

var checkSettings = new CheckSettings {
    GitRoot             = root,
    StagedChanges       = checkStaged,
    UncommittedChanges  = checkUncommitted,
    UntrackedFiles      = checkUntracked,
    RequireReleaseNotes = requireReleaseNotes
};

var restoreSettings = new DotNetCoreRestoreSettings {};

var msBuildSettings = new DotNetCoreMSBuildSettings {};

var buildSettings = new DotNetCoreBuildSettings {
    Configuration       = configuration,
    MSBuildSettings     = msBuildSettings,
    NoRestore           = true
};

var publishSettings = new DotNetCorePublishSettings {
    Configuration       = configuration,
    NoBuild             = true,
    NoRestore           = true,
    OutputDirectory     = $"{publishDir}"
};

var packSettings = new NuGetPackSettings {
    Id                  = solution,
    Title               = solution,
    Authors             = new [] { "Jonathan Higgs <jonathan.higgs.11@mail.wbs.ac.uk>" },
    Owners              = new [] { "Jonathan Higgs <jonathan.higgs.11@mail.wbs.ac.uk>" },
    Description         = "Cake build tools for automated .Net builds",
    Summary             = "Cake build tools for automated .Net builds",
    ProjectUrl          = new Uri("https://github.com/JonathanHiggs/Higgsoft.Cake"),
    Copyright           = $"Copyright (c) Jonathan Higgs {DateTime.Today.Year}",
    Tags                = new [] { "Cake", "Build", "Check", "Version", "Release", "Notes" },
    Symbols             = false,
    BasePath            = publishDir,
    OutputDirectory     = nugetDir,
    RequireLicenseAcceptance = false,
    Properties = new Dictionary<string, string> {
        { "Configuration", configuration }
    },
    Dependencies        = new [] {      // This is horrible
        new NuSpecDependency { Id = "Cake.Common", Version = "0.37.0", TargetFramework = "net48" },
        new NuSpecDependency { Id = "Cake.Core", Version = "0.37.0", TargetFramework = "net48" },
        new NuSpecDependency { Id = "Cake.Git", Version = "0.21.0", TargetFramework = "net48" },
        new NuSpecDependency { Id = "Cake.Common", Version = "0.37.0", TargetFramework = "netstandard2.0" },
        new NuSpecDependency { Id = "Cake.Core", Version = "0.37.0", TargetFramework = "netstandard2.0" },
        new NuSpecDependency { Id = "Cake.Git", Version = "0.21.0", TargetFramework = "netstandard2.0" },
        new NuSpecDependency { Id = "Cake.Common", Version = "0.37.0", TargetFramework = "netcoreapp3.1" },
        new NuSpecDependency { Id = "Cake.Core", Version = "0.37.0", TargetFramework = "netcoreapp3.1" },
        new NuSpecDependency { Id = "Cake.Git", Version = "0.21.0", TargetFramework = "netcoreapp3.1" },
    }
};

var pushSettings = new NuGetPushSettings {
    Source = "Local"
};

var frameworks = new [] { "net48", "netstandard2.0", "netcoreapp3.1" };

var version = new Higgsoft.Cake.Versions.Version(0, 0, 1);


//////////////////////////
// Setup & Teardown
////////////////////////

Setup(context => {
    EnsureDirectoryExists(publishDir);
    EnsureDirectoryExists(nugetDir);
});

Teardown(context => { });


//////////////////////////
// Tasks
////////////////////////

Task("Info")
    .Does(() => {
        Information( "-- Arguments");
        Information($"target                    {target}");
        Information($"configuration             {configuration}");
        Information($"check-staged              {checkStaged}");
        Information($"check-uncommitted         {checkUncommitted}");
        Information($"check-untracked           {checkUntracked}");
        Information($"require-release-notes     {requireReleaseNotes}");
        Information("\n-- Files & Directories");
        Information($"solution                  {solution}");
        Information($"project                   {project}");
        Information($"root                      {root}");
        Information($"solutionDir               {solutionDir}");
        Information($"projectDir                {projectDir}");
        Information($"publishDir                {publishDir}");
        Information($"nugetDir                  {nugetDir}");
        Information($"solutionFile              {solutionFile}");
        Information($"projectFile               {projectFile}");
        Information($"releaseNotes              {releaseNotes}");
        Information($"releaseNotesVNext         {releaseNotesVNext}");
    });


Task("Check")
    .IsDependentOn("Info")
    .Does(() => Check(checkSettings));


Task("Version")
    .IsDependentOn("Check")
    .Does(() => {
        version = ParseAndUpdateVersion(
            releaseNotesVNext,
            dotNetCoreMSBuildSettings: msBuildSettings,
            nuGetPackSettings: packSettings);

        // Update recipe addin version numbers
        ReplaceRegexInFiles(
            "./**/*recipe.cake",
            "Higgsoft\\.Cake&version=\\d+\\.\\d+\\.\\d+",
            $"Higgsoft.Cake&version={version.ToString()}");

        // Update recipe test build script version numbers
        ReplaceRegexInFiles(
            "./test/**/build.cake",
            "Higgsoft\\.Cake&version=\\d+\\.\\d+\\.\\d+",
            $"Higgsoft.Cake&version={version.ToString()}");

        Information(version.ToString());
    });


Task("Clean")
    .IsDependentOn("Version")
    .Does(() => {
        DotNetCoreClean(solutionFile);
        CleanDirectory(publishDir);
    });


Task("Restore")
    .IsDependentOn("Clean")
    .Does(() => DotNetCoreRestore(solutionFile, restoreSettings));


Task("Build")
    .IsDependentOn("Restore")
    .DoesForEach(frameworks, framework => {
        buildSettings.Framework = framework;
        DotNetCoreBuild(solutionFile, buildSettings);
    });


Task("Publish")
    .IsDependentOn("Build")
    .DoesForEach(frameworks, framework => {
        publishSettings.Framework = framework;
        publishSettings.OutputDirectory = $"{publishDir}/{framework}";

        DotNetCorePublish(projectFile, publishSettings);
    });


Task("Package")
    .IsDependentOn("Publish")
    .Does(() => {
        var assemblyFiles =
            GetFiles(publishDir + $"/**/{project}.dll")
                .Union(GetFiles(publishDir + $"/**/{project}.pdb"))
                .Where(f => f.FullPath.Contains($"{project}.") && !f.FullPath.Contains("/runtimes/"))
                .Select(f => f.FullPath.Substring(publishDir.FullPath.Length + 1))
                .Select(f => new NuSpecContent { Source = f, Target = $"lib/{f}" });

        var recipeScripts =
            GetFiles("./src/**/*recipe.cake")
                .Select(f => MakeAbsolute(f))
                .Select(f => new NuSpecContent { Source = f.FullPath, Target = $"content/{f.GetFilename()}" });

        packSettings.Files =
            assemblyFiles
            .Union(recipeScripts)
            .ToArray();

        NuGetPack(packSettings);
    });


Task("AliasTests")
    .IsDependentOn("Package")
    .WithCriteria(() => runTests)
    .Does(() => {
        // Manually install package
        var package = File($"{nugetDir}/{project}.{version}.nupkg");
        var addinDir = Directory($"{root}/tools/Addins/{project}/{project}");

        if (DirectoryExists(addinDir))
            DeleteDirectory(addinDir, new DeleteDirectorySettings { Recursive = true, Force = true });

        Unzip(package, addinDir);

        // Run tests
        Information("Test - net48");
        CakeExecuteScript("./test-net48.cake");

        Information("Test - netstandard20");
        DotNetCoreExecute("./tools/Cake.CoreCLR/Cake.dll", "test-netstandard20.cake");

        // Clean up
        DeleteDirectory(
            Directory($"{root}/tools/Addins/{project}"),
            new DeleteDirectorySettings { Recursive = true, Force = true });
    });


Task("RecipeTests")
    .IsDependentOn("AliasTests")
    .WithCriteria(() => runTests)
    .Does(() => {
        // Manually install package
        var dirs = new[] { root, $"{root}/test/dotnet-lib", $"{root}/test/dotnet-app" };
        var package = File($"{nugetDir}/{project}.{version}.nupkg");

        foreach (var dir in dirs)
        {
            var addinDir = Directory($"{dir}/tools/Addins/{project}.{version}");

            if (DirectoryExists(addinDir))
                DeleteDirectory(addinDir, new DeleteDirectorySettings { Recursive = true, Force = true });

            Unzip(package, addinDir);
            CopyFile(package, File($"{addinDir}/{project}.{version}.nupkg"));

            var toolsDir = Directory($"{dir}/tools/{project}.{version}");

            if (DirectoryExists(toolsDir))
                DeleteDirectory(toolsDir, new DeleteDirectorySettings { Recursive = true, Force = true });

            Unzip(package, toolsDir);
            CopyFile(package, File($"{toolsDir}/{project}.{version}.nupkg"));
        }

        // Run tests
        CakeExecuteScript(
            "./test/dotnet-lib/build.cake",
            new CakeSettings {
                Arguments = new Dictionary<string, string>{
                    { "local", "true" },
                    { "check-staged", "false" },
                    { "check-uncommitted", "false" },
                    { "enable-commits", "false" },
                    { "enable-push", "false" } },
                WorkingDirectory = Directory("./test/dotnet-lib") });

        CakeExecuteScript(
            "./test/dotnet-app/build.cake",
            new CakeSettings {
                Arguments = new Dictionary<string, string>{
                    { "local", "true" },
                    { "check-staged", "false" },
                    { "check-uncommitted", "false" },
                    { "enable-commits", "false" },
                    { "enable-push", "false" } },
                WorkingDirectory = Directory("./test/dotnet-app") });

        // Clean up
        foreach (var dir in dirs)
        {
            var addinDir = Directory($"{dir}/tools/Addins/{project}.{version}");
            var toolsDir = Directory($"{dir}/tools/{project}.{version}");

            DeleteDirectory(addinDir, new DeleteDirectorySettings { Recursive = true, Force = true });
            DeleteDirectory(toolsDir, new DeleteDirectorySettings { Recursive = true, Force = true });
        }
    });


Task("Push")
    .IsDependentOn("AliasTests")
    .Does(() => {
        DotNetCoreNuGetDelete(
            project,
            version.ToString(),
            new DotNetCoreNuGetDeleteSettings {
                Source = "Local",
                NonInteractive = true });

        var package = File($"{nugetDir}/{project}.{version}.nupkg");
        NuGetPush(package, pushSettings);
    });


//////////////////////////
// Targets
////////////////////////

Task("Default")
    .IsDependentOn("RecipeTests")
    .Does(() => Information("Default build completed"));


//////////////////////////
// Invoke
////////////////////////

RunTarget(target);