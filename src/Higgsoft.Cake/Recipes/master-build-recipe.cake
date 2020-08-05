//////////////////////
// Includes & Using
////////////////////

#addin nuget:?package=Higgsoft.Cake&version=0.1.0
#addin nuget:?package=Cake.Git&version=0.21.0

using Higgsoft.Cake.Recipes;


//////////////////////
// Arguments
////////////////////

Build.Target                    = Argument("target",                Build.Target);
Build.Configuration             = Argument("configuration",         Build.Configuration);
Build.Verbosity                 = Argument("verbosity",             Build.Verbosity);
Build.Local                     = Argument("local",                 Build.Local);

Build.CheckStagedChanges        = Argument("check-staged",          Build.CheckStagedChanges);
Build.CheckUncommittedChanges   = Argument("check-uncommitted",     Build.CheckUncommittedChanges);
Build.CheckUntrackedFiles       = Argument("check-untracked",       Build.CheckUntrackedFiles);

Build.GitRoot                   = GitFindRootFromPath(MakeAbsolute(Directory(".")));
Build.GitUserName               = Argument("git-username",          Build.GitUserName);
Build.GitEmail                  = Argument("git-email",             Build.GitEmail);
Build.GitRemoteName             = Argument("git-remote",            Build.GitRemoteName);
Build.EnableCommits             = Argument("enable-commits",        Build.EnableCommits);
Build.EnableTags                = Argument("enable-tags",           Build.EnableTags);
Build.EnablePush                = Argument("enable-push",           Build.EnablePush);

Build.NuGetSource               = Argument("nuget-source",          Build.NuGetSource);
Build.NuGetLocalSource          = Argument("nuget-local-source",    Build.NuGetLocalSource);
Build.NuGetApiKey               = Argument("nuget-api-key",         EnvironmentVariable("NUGET_API_KEY"));

Build.ArtefactsRepository       = Argument("artefacts-repo",        Build.ArtefactsRepository);
Build.ArtefactsLocalRepository  = Argument("artefacts-local-repo",  Build.ArtefactsLocalRepository);

Build.SquirrelCentralRepository = Argument("squirrel-repo",         Build.SquirrelCentralRepository);
Build.SquirrelLocalRepository   = Argument("squirrel-local-repo",   Build.SquirrelLocalRepository);


//////////////////////
// Tasks
////////////////////

Build.Tasks.Info = Task("Build-Info")
    .Does(() => BuildInfo());

Build.Tasks.Check = Task("Build-Check")
    .IsDependentOn(Build.Tasks.Info)
    .Does(() => Check(Build.CheckSettings));

Build.Tasks.Push = Task("Build-Push")
    .WithCriteria(() => Build.EnableCommits && Build.EnablePush && !Build.Local)
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