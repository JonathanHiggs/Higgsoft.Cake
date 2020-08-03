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
        .IsDependentOn(Build.PreBuild)
        .IsDependeeOf(Build.InfoOnly.Task.Name)
        .Does(() => DotNetLibInfo(lib));

    tasks.Setup = Task($"{lib.Id}-Setup")
        .IsDependentOn(tasks.Info)
        .Does(() => DotNetLibSetup(lib))
        .OnError(ex => DotNetLibOnError(lib, tasks.Setup, ex));

    tasks.Check = Task($"{lib.Id}-Check")
        .IsDependentOn(tasks.Setup)
        .WithCriteria(() => !lib.Errored)
        .Does(() => DotNetLibCheck(lib))
        .OnError(ex => DotNetLibOnError(lib, tasks.Check, ex));

    tasks.Version = Task($"{lib.Id}-Version")
        .IsDependentOn(tasks.Check)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .Does(() => DotNetLibVersion(lib))
        .OnError(ex => DotNetLibOnError(lib, tasks.Version, ex));

    tasks.ReleaseNotes = Task($"{lib.Id}-ReleaseNotes")
        .IsDependentOn(tasks.Version)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored && lib.PrepareReleaseNotes)
        .Does(() => DotNetLibReleaseNotes(lib))
        .OnError(ex => DotNetLibOnError(lib, tasks.ReleaseNotes, ex));

    tasks.AssemblyInfo = Task($"{lib.Id}-AssemblyInfo")
        .IsDependentOn(tasks.ReleaseNotes)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored && lib.UpdateAssemblyInfo)
        .Does(() => DotNetLibAssemblyInfo(lib))
        .OnError(ex => DotNetLibOnError(lib, tasks.AssemblyInfo, ex));

    tasks.Clean = Task($"{lib.Id}-Clean")
        .IsDependentOn(tasks.AssemblyInfo)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .Does(() => DotNetLibClean(lib))
        .OnError(ex => DotNetLibOnError(lib, tasks.Clean, ex));

    tasks.Restore = Task($"{lib.Id}-Restore")
        .IsDependentOn(tasks.Clean)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .Does(() => DotNetLibRestore(lib))
        .OnError(ex => DotNetLibOnError(lib, tasks.Restore, ex));

    tasks.PreBuild = Task($"{lib.Id}-PreBuild")
        .IsDependentOn(tasks.Restore)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored && lib.UsePreBuildTask)
        .OnError(ex => DotNetLibOnError(lib, tasks.PreBuild, ex));

    tasks.Build = Task($"{lib.Id}-Build")
        .IsDependentOn(tasks.PreBuild)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .Does(() => DotNetLibBuild(lib))
        .OnError(ex => DotNetLibOnError(lib, tasks.Build, ex));

    tasks.PostBuild = Task($"{lib.Id}-PostBuild")
        .IsDependentOn(tasks.Build)
        .IsDependeeOf(Build.BuildAll.Task.Name)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored && lib.UsePostBuildTask)
        .OnError(ex => DotNetLibOnError(lib, tasks.PostBuild, ex));

    tasks.Test = Task($"{lib.Id}-Test")
        .IsDependentOn(tasks.PostBuild)
        .IsDependeeOf(Build.TestAll.Task.Name)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .Does(() => DotNetLibTest(lib))
        .OnError(ex => DotNetLibOnError(lib, tasks.Test, ex));

    tasks.Publish = Task($"{lib.Id}-Publish")
        .IsDependentOn(tasks.Test)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .DoesForEach(lib.PublishSettings, settings => DotNetLibPublish(lib, settings))
        .OnError(ex => DotNetLibOnError(lib, tasks.Publish, ex));

    tasks.Package = Task($"{lib.Id}-Package")
        .IsDependentOn(tasks.Publish)
        .IsDependeeOf(Build.PackageAll.Task.Name)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .Does(() => DotNetLibPackage(lib))
        .OnError(ex => DotNetLibOnError(lib, tasks.Package, ex));

    tasks.Commit = Task($"{lib.Id}-Commit")
        .IsDependentOn(tasks.Package)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored && !Build.Local)
        .Does(() => DotNetLibCommit(lib))
        .OnError(ex => DotNetLibOnError(lib, tasks.Commit, ex));

    tasks.Push = Task($"{lib.Id}-Push")
        .IsDependentOn(tasks.Commit)
        .WithCriteria(() => !lib.SkipRemainingTasks && !lib.Errored)
        .Does(() => DotNetLibPush(lib))
        .OnError(ex => DotNetLibOnError(lib, tasks.Push, ex));

    tasks.CleanUp = Task($"{lib.Id}-CleanUp")
        .IsDependentOn(tasks.Push)
        .IsDependeeOf(Build.RunAll.Task.Name)
        .WithCriteria(() => lib.Errored || lib.SkipRemainingTasks || Build.Local)
        .Does(() => DotNetLibCleanUp(lib))
        .OnError(ex => DotNetLibOnError(lib, tasks.CleanUp, ex));
};


Func<Action<DotNetLib>, DotNetLib> AddDotNetLib = (Action<DotNetLib> config) => {
    var lib = ConfigDotNetLib(config);
    SetDotNetLibTasks(lib);
    return lib;
};