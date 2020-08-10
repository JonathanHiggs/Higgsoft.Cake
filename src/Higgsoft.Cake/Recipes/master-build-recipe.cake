//////////////////////
// Includes & Using
////////////////////

#addin nuget:?package=Higgsoft.Cake&version=0.1.0
#addin nuget:?package=Cake.Git&version=0.21.0

using Higgsoft.Cake.Recipes;


//////////////////////
// Arguments
////////////////////

BuildConfigure();


//////////////////////
// Tasks
////////////////////

Build.Tasks.Info = Task("Build-Info")
    .Does(() => BuildInfo());

Build.Tasks.Check = Task("Build-Check")
    .IsDependentOn(Build.Tasks.Info)
    .Does(() => Check(Build.CheckSettings));

Build.Tasks.Push = Task("Build-Push")
    .WithCriteria(() => Build.ShouldPush)
    .Does(() => BuildPush());


//////////////////////
// Targets
////////////////////

Build.Targets.InfoOnly = Task("Build-InfoOnly")
    .IsDependentOn(Build.Tasks.Info)
    .Does(() => BuildStatus());

Build.Targets.BuildAll = Task("Build-BuildAll")
    .Does(() => BuildStatus());

Build.Targets.TestAll = Task("Build-TestAll")
    .Does(() => BuildStatus());

Build.Targets.PackageAll = Task("Build-PackageAll")
    .Does(() => BuildStatus());

Build.Targets.RunAll = Task("Build-RunAll")
    .IsDependentOn(Build.Tasks.Push)
    .Does(() => BuildStatus());