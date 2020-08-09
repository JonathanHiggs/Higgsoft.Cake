//////////////////////
// Includes & Using
////////////////////

#tool nuget:?package=NUnit.ConsoleRunner&version=3.9.0

#addin nuget:?package=Higgsoft.Cake&version=0.1.0

using Higgsoft.Cake.Recipes;
using Higgsoft.Cake.Recipes.Libs;


//////////////////////
// Task Factory
////////////////////

Action<DotNetLib> SetDotNetLibTasks = (DotNetLib lib) => {
    var tasks = lib.Tasks;
    var names = tasks.Names;

    tasks.Info = Task(names.Info)
        .ConfigTaskFor(lib, Build.Tasks.Check.Task.Name)
        .Does(() => DotNetLibInfo(lib));

    tasks.Setup = Task(names.Setup)
        .ConfigTaskFor(lib, names.Info)
        .Does(() => DotNetLibSetup(lib));

    tasks.Check = Task(names.Check)
        .ConfigTaskFor(lib, names.Setup)
        .Does(() => RecipeCheck(lib));

    tasks.Version = Task(names.Version)
        .ConfigTaskFor(lib, names.Check)
        .Does(() => RecipeVersion(lib));

    if (lib.PrepareReleaseNotes)
        tasks.ReleaseNotes = Task(names.ReleaseNotes)
            .ConfigTaskFor(lib, names.Version)
            .Does(() => RecipeReleaseNotes(lib));

    if (lib.UpdateAssemblyInfo)
        tasks.AssemblyInfo = Task(names.AssemblyInfo)
            .ConfigTaskFor(lib, names.ReleaseNotes)
            .Does(() => RecipeAssemblyInfo(lib));

    tasks.Clean = Task(names.Clean)
        .ConfigTaskFor(lib, names.AssemblyInfo)
        .Does(() => DotNetLibClean(lib));

    if (lib.UsePreBuildTask)
        tasks.PreBuild = Task(names.PreBuild)
            .ConfigTaskFor(lib, names.Clean);;

    tasks.Build = Task(names.Build)
        .ConfigTaskFor(lib, names.PreBuild)
        .DoesForEach(lib.RestoreBuildSettings, settings => DotNetLibBuild(lib, settings));

    if (lib.UsePostBuildTask)
        tasks.PostBuild = Task(names.PostBuild)
            .ConfigTaskFor(lib, names.Build);

    tasks.Test = Task(names.Test)
        .ConfigTaskFor(lib, names.PostBuild)
        .Does(() => RecipeTest(lib));

    tasks.Publish = Task(names.Publish)
        .ConfigTaskFor(lib, names.Test)
        .DoesForEach(lib.RestorePublishSettings, settings => DotNetLibPublish(lib, settings));

    tasks.Package = Task(names.Package)
        .ConfigTaskFor(lib, names.Publish)
        .Does(() => DotNetLibPackage(lib));

    if (lib.UseCommitTask)
        tasks.Commit = Task(names.Commit)
            .ConfigTaskFor(lib, names.Package)
            .Does(() => RecipeCommit(lib));
            
    tasks.Push = Task(names.Push)
        .ConfigTaskFor(lib, names.Commit)
        .Does(() => DotNetLibPush(lib));

    // ToDo: manually setup the cleanup so it doesn't skip
    tasks.CleanUp = Task(names.CleanUp)
        .ConfigTaskFor(
            lib,
            DotNetLibCleanUpDependency(lib),
            DotNetLibCleanUpDependee(lib))
        .Does(() => RecipeCleanUp(lib));
};


Func<Action<DotNetLib>, DotNetLib> AddDotNetLib = (Action<DotNetLib> config) => {
    var lib = ConfigDotNetLib(config);
    SetDotNetLibTasks(lib);
    return lib;
};