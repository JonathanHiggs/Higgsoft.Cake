//////////////////////////
// Includes & Using
////////////////////////

#load nuget:?package=Higgsoft.Cake&version=0.1.0


//////////////////////////
// Definitions
////////////////////////

AddDotNetLib(lib => {
    lib.Id                          = "DotNetLibTest";
    lib.Name                        = "DotNetLibTest";
    lib.Description                 = "dotnet lib test";
    lib.Solution                    = "DotNetLibTest";
    lib.Project                     = "DotNetLibTest";
    lib.Guid                        = "";


    lib.SolutionDirectory           = Directory($"{Build.GitRoot}/test/dotnet-lib");
    lib.SolutionFile                = File($"{lib.SolutionDirectory}/{lib.Solution}.sln");
    lib.ProjectFile                 = File($"{lib.SolutionDirectory}/{lib.Project}/{lib.Project}.csproj");
    lib.AssemblyInfoFile            = null;
    lib.ReleaseNotesFile            = File($"{lib.SolutionDirectory}/ReleaseNotes.md");
    lib.ReleaseNotesVNextFile       = File($"{lib.SolutionDirectory}/ReleaseNotes.vnext.md");;
    //lib.BuildDirectory              = null;
    lib.PublishDirectory            = Directory($"{lib.SolutionDirectory}/publish");
    lib.NuGetDirectory              = Directory($"{lib.SolutionDirectory}/nuget");

    lib.SharedAssemblyInfoFile      = false;
    lib.UsePreBuildTask             = false;
    lib.UsePostBuildTask            = false;
    lib.PrepareReleaseNotes         = true;
    lib.UpdateAssemblyInfo          = false;
    lib.CommitChanges               = false;
    lib.TagVersion                  = false;
    lib.PushToRemote                = false;

    lib.ProjectUrl                  = new Uri("https://github.com/JonathanHiggs/Higgsoft.Cake");

    lib.AddFrameworks("netstandard2.0", "netcoreapp3.1");
    lib.AddAuthors("Jonathan Higgs");
});


//////////////////////////
// Invoke
////////////////////////

RunTarget(Build.Target);
