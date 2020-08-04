//////////////////////////
// Includes & Using
////////////////////////

#load nuget:?package=Higgsoft.Cake&version=0.1.0


//////////////////////////
// Definitions
////////////////////////

AddDotNetApp(app => {
    app.Id                          = "DotNetAppTest";
    app.Name                        = "DotNetAppTest";
    app.Description                 = "dotnet app test";
    app.Solution                    = "DotNetAppTest";
    app.Project                     = "DotNetAppTest";
    app.Guid                        = "";


    app.SolutionDirectory           = Directory($"{Build.GitRoot}/test/dotnet-app");
    app.SolutionFile                = File($"{app.SolutionDirectory}/{app.Solution}.sln");
    app.ProjectFile                 = File($"{app.SolutionDirectory}/{app.Project}/{app.Project}.csproj");
    app.AssemblyInfoFile            = null;
    app.ReleaseNotesFile            = File($"{app.SolutionDirectory}/ReleaseNotes.md");
    app.ReleaseNotesVNextFile       = File($"{app.SolutionDirectory}/ReleaseNotes.vnext.md");;
    app.PublishDirectory            = Directory($"{app.SolutionDirectory}/publish");
    app.PackageDirectory            = Directory($"{app.SolutionDirectory}/package");
    app.ArtefactDirectory           = Directory($"{app.SolutionDirectory}/artefact");

    app.SharedAssemblyInfoFile      = false;
    app.UsePreBuildTask             = false;
    app.UsePostBuildTask            = false;
    app.PrepareReleaseNotes         = true;
    app.UpdateAssemblyInfo          = false;
    app.CommitChanges               = false;
    app.TagVersion                  = false;
    app.PushToRemote                = false;

    app.AddFrameworks("netcoreapp3.1");
    app.AddRuntimes("win10-x64", "linux-x64", "linux-arm64");
});


//////////////////////////
// Invoke
////////////////////////

RunTarget(Build.Target);
