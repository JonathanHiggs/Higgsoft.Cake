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

    tasks.Info = Task($"{lib.Id}-Info")
        .ConfigTaskFor(lib, Build.Tasks.Check)
        .Does(() => DotNetLibInfo(lib));

    tasks.Setup = Task($"{lib.Id}-Setup")
        .ConfigTaskFor(lib, tasks.Info)
        .Does(() => DotNetLibSetup(lib));

    tasks.Check = Task($"{lib.Id}-Check")
        .ConfigTaskFor(lib, tasks.Setup)
        .Does(() => RecipeCheck(lib));

    tasks.Version = Task($"{lib.Id}-Version")
        .ConfigTaskFor(lib, tasks.Check)
        .Does(() => RecipeVersion(lib));

    if (lib.PrepareReleaseNotes)
        tasks.ReleaseNotes = Task($"{lib.Id}-ReleaseNotes")
            .ConfigTaskFor(lib, tasks.Version)
            .Does(() => RecipeReleaseNotes(lib));

    if (lib.UpdateAssemblyInfo)
        tasks.AssemblyInfo = Task($"{lib.Id}-AssemblyInfo")
            .ConfigTaskFor(lib, tasks.ReleaseNotes)
            .Does(() => RecipeAssemblyInfo(lib));

    tasks.Clean = Task($"{lib.Id}-Clean")
        .ConfigTaskFor(lib, tasks.AssemblyInfo)
        .Does(() => DotNetLibClean(lib));

    if (lib.UsePreBuildTask)
        tasks.PreBuild = Task($"{lib.Id}-PreBuild")
            .ConfigTaskFor(lib, tasks.Clean);;

    tasks.Build = Task($"{lib.Id}-Build")
        .ConfigTaskFor(lib, tasks.PreBuild)
        .DoesForEach(lib.RestoreBuildSettings, settings => DotNetLibBuild(lib, settings));

    if (lib.UsePostBuildTask)
        tasks.PostBuild = Task($"{lib.Id}-PostBuild")
            .ConfigTaskFor(lib, tasks.Build);

    tasks.Test = Task($"{lib.Id}-Test")
        .ConfigTaskFor(lib, tasks.PostBuild)
        .Does(() => RecipeTest(lib));

    tasks.Publish = Task($"{lib.Id}-Publish")
        .ConfigTaskFor(lib, tasks.Test)
        .DoesForEach(lib.RestorePublishSettings, settings => DotNetLibPublish(lib, settings));

    tasks.Package = Task($"{lib.Id}-Package")
        .ConfigTaskFor(lib, tasks.Publish)
        .Does(() => DotNetLibPackage(lib));

    if (lib.UseCommitTask)
        tasks.Commit = Task($"{lib.Id}-Commit")
            .ConfigTaskFor(lib, tasks.Package)
            .Does(() => RecipeCommit(lib));
            
    tasks.Push = Task($"{lib.Id}-Push")
        .ConfigTaskFor(lib, tasks.Commit)
        .Does(() => DotNetLibPush(lib));

    tasks.CleanUp = Task($"{lib.Id}-CleanUp")
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