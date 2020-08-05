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
        .IsDependentOn(Build.Tasks.Check)
        .IsDependeeOf(Build.Targets.InfoOnly.Task.Name)
        .Does(() => DotNetLibInfo(lib));

    tasks.Setup = Task($"{lib.Id}-Setup")
        .IsDependentOn(tasks.Info)
        .Does(() => DotNetLibSetup(lib))
        .OnError(ex => RecipeOnError(lib, tasks.Setup, ex));

    tasks.Check = Task($"{lib.Id}-Check")
        .IsDependentOn(tasks.Setup)
        .WithCriteria(() => !lib.Errored)
        .Does(() => RecipeCheck(lib))
        .OnError(ex => RecipeOnError(lib, tasks.Check, ex));

    tasks.Version = Task($"{lib.Id}-Version")
        .IsDependentOn(tasks.Check)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .Does(() => RecipeVersion(lib))
        .OnError(ex => RecipeOnError(lib, tasks.Version, ex));

    tasks.ReleaseNotes = Task($"{lib.Id}-ReleaseNotes")
        .IsDependentOn(tasks.Version)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored && lib.PrepareReleaseNotes)
        .Does(() => RecipeReleaseNotes(lib))
        .OnError(ex => RecipeOnError(lib, tasks.ReleaseNotes, ex));

    if (lib.UpdateAssemblyInfo)
        tasks.AssemblyInfo = Task($"{lib.Id}-AssemblyInfo")
            .IsDependentOn(tasks.ReleaseNotes)
            .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
            .Does(() => RecipeAssemblyInfo(lib))
            .OnError(ex => RecipeOnError(lib, tasks.AssemblyInfo, ex));

    tasks.Clean = Task($"{lib.Id}-Clean")
        .IsDependentOn(lib.UpdateAssemblyInfo ? tasks.AssemblyInfo : tasks.ReleaseNotes)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .Does(() => DotNetLibClean(lib))
        .OnError(ex => RecipeOnError(lib, tasks.Clean, ex));

    if (lib.UsePreBuildTask)
        tasks.PreBuild = Task($"{lib.Id}-PreBuild")
            .IsDependentOn(tasks.Clean)
            .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
            .OnError(ex => RecipeOnError(lib, tasks.PreBuild, ex));

    tasks.Build = Task($"{lib.Id}-Build")
        .IsDependentOn(lib.UsePreBuildTask ? tasks.PreBuild : tasks.Clean)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .DoesForEach(lib.RestoreBuildSettings, settings => DotNetLibBuild(lib, settings))
        .OnError(ex => RecipeOnError(lib, tasks.Build, ex));

    if (lib.UsePostBuildTask)
        tasks.PostBuild = Task($"{lib.Id}-PostBuild")
            .IsDependentOn(tasks.Build)
            .IsDependeeOf(Build.Targets.BuildAll.Task.Name)
            .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
            .OnError(ex => RecipeOnError(lib, tasks.PostBuild, ex));

    tasks.Test = Task($"{lib.Id}-Test")
        .IsDependentOn(lib.UsePostBuildTask ? tasks.PostBuild : tasks.Build)
        .IsDependeeOf(Build.Targets.TestAll.Task.Name)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .Does(() => RecipeTest(lib))
        .OnError(ex => RecipeOnError(lib, tasks.Test, ex));

    tasks.Publish = Task($"{lib.Id}-Publish")
        .IsDependentOn(tasks.Test)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .DoesForEach(lib.RestorePublishSettings, settings => DotNetLibPublish(lib, settings))
        .OnError(ex => RecipeOnError(lib, tasks.Publish, ex));

    tasks.Package = Task($"{lib.Id}-Package")
        .IsDependentOn(tasks.Publish)
        .IsDependeeOf(Build.Targets.PackageAll.Task.Name)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .Does(() => DotNetLibPackage(lib))
        .OnError(ex => RecipeOnError(lib, tasks.Package, ex));

    if (!Build.Local)
        tasks.Commit = Task($"{lib.Id}-Commit")
            .IsDependentOn(tasks.Package)
            .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
            .Does(() => RecipeCommit(lib))
            .OnError(ex => RecipeOnError(lib, tasks.Commit, ex));

    tasks.Push = Task($"{lib.Id}-Push")
        .IsDependentOn(!Build.Local ? tasks.Commit : tasks.Package)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .Does(() => DotNetLibPush(lib))
        .OnError(ex => RecipeOnError(lib, tasks.Push, ex));

    tasks.CleanUp = Task($"{lib.Id}-CleanUp")
        .IsDependentOn(tasks.Push)
        .IsDependeeOf(Build.Tasks.Push.Task.Name)
        .Does(() => RecipeCleanUp(lib))
        .OnError(ex => RecipeOnError(lib, tasks.CleanUp, ex));
};


Func<Action<DotNetLib>, DotNetLib> AddDotNetLib = (Action<DotNetLib> config) => {
    var lib = ConfigDotNetLib(config);
    SetDotNetLibTasks(lib);
    return lib;
};