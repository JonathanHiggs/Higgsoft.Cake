using System;
using System.Collections.Generic;

using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;

using Higgsoft.Cake.Check;

namespace Higgsoft.Cake.Recipes
{
    /// <summary>
    /// Global build settings used to coordinate information between the main build and sub-build
    /// recipes such as build configuration or paths to external resources
    /// </summary>
    public static class Build
    {
        #region Global

        /// <summary>
        /// Gets and sets the build configuration
        /// </summary>
        public static string Configuration { get; set; } = "Release";


        /// <summary>
        /// Gets and sets the build target
        /// </summary>
        public static string Target { get; set; } = "RunAll";


        /// <summary>
        /// Gets and sets the amount of logging
        /// </summary>
        public static Verbosity Verbosity { get; set; } = Verbosity.Quiet;


        /// <summary>
        /// Gets and sets a flag that determines whether the build is local
        /// </summary>
        public static bool Local { get; set; } = true;


        /// <summary>
        /// Gets and sets the company name
        /// </summary>
        public static string Company { get; set; } = string.Empty;

        #endregion


        #region Checks

        /// <summary>
        /// Gets and sets a flag that determines whether the build will check for staged file
        /// changes in the git repository
        /// </summary>
        public static bool CheckStagedChanges { get; set; } = true;


        /// <summary>
        /// Gets and sets a flag that determines whether the build will check for uncommitted file
        /// changes in the git repository
        /// </summary>
        public static bool CheckUncommittedChanges { get; set; } = true;


        /// <summary>
        /// Gets and sets a flag that determines whether the build will check for untracked file
        /// changes in the git repository
        /// </summary>
        public static bool CheckUntrackedFiles { get; set; } = true;


        public static CheckSettings CheckSettings
            => new CheckSettings {
                GitRoot = GitRoot,
                StagedChanges = CheckStagedChanges,
                UncommittedChanges = CheckUncommittedChanges,
                UntrackedFiles = CheckUntrackedFiles,
                // Checked at the recipe level
                RequireReleaseNotes = false
            };

        #endregion


        #region Git

        /// <summary>
        /// Gets and sets the path to the git repository root
        /// </summary>
        public static DirectoryPath GitRoot { get; set; }


        /// <summary>
        /// Gets and sets the git user name
        /// </summary>
        public static string GitUserName { get; set; } = "CakeBuild";


        /// <summary>
        /// Gets and sets the git email address
        /// </summary>
        public static string GitEmail { get; set; } = "cake@build.com";


        /// <summary>
        /// Gets and sets a flag that enables git repository commits
        /// </summary>
        public static bool EnableCommits { get; set; } = true;


        /// <summary>
        /// Gets and sets a flag that enables git repository version tags
        /// </summary>
        public static bool EnableTags { get; set; } = true;

        #endregion


        #region NuGet

        /// <summary>
        /// Gets and sets the NuGet local package source
        /// </summary>
        public static string NuGetLocalSource { get; set; }
            = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\.nuget\\packages";


        /// <summary>
        /// Gets and sets the NuGet package source
        /// </summary>
        public static string NuGetSource { get; set; } = "https://api.nuget.org/v3/index.json";


        /// <summary>
        /// Gets and sets the NuGet api key
        /// </summary>
        public static string NuGetApiKey { get; set; }

        #endregion


        #region Squirrel

        /// <summary>
        /// Gets and sets the path to the central squirrel releases repository
        /// </summary>
        public static DirectoryPath SquirrelCentralRepository { get; set; }


        /// <summary>
        /// Gets and sets the path to the local squirrel releases repository
        /// </summary>
        public static DirectoryPath SquirrelLocalRepository { get; set; }
            = new DirectoryPath(".\\releases");


        /// <summary>
        /// Returns the path to the squirrel releases repository dependent on whether the build is
        /// set to local or not
        /// </summary>
        /// <param name="id">Package identity</param>
        /// <returns>Path to the squirrel releases repository</returns>
        public static DirectoryPath SquirrelRepository(string id)
            => !Local
            ? new DirectoryPath($"{SquirrelCentralRepository}\\{id}")
            : SquirrelLocalRepository;

        #endregion


        #region Tasks

        /// <summary>
        /// Gets and sets the info task builder
        /// </summary>
        public static CakeTaskBuilder Info { get; set; }


        /// <summary>
        /// Gets and sets the info-only task builder
        /// </summary>
        public static CakeTaskBuilder InfoOnly { get; set; }


        /// <summary>
        /// Gets and sets the pre-build task builder
        /// </summary>
        public static CakeTaskBuilder PreBuild { get; set; }


        /// <summary>
        /// Gets and sets the build-all task builder
        /// </summary>
        public static CakeTaskBuilder BuildAll { get; set; }


        /// <summary>
        /// Gets and sets the test-all task builder
        /// </summary>
        public static CakeTaskBuilder TestAll { get; set; }


        /// <summary>
        /// Gets and sets the package-all task builder
        /// </summary>
        public static CakeTaskBuilder PackageAll { get; set; }


        /// <summary>
        /// Gets and sets the run-all task builder
        /// </summary>
        public static CakeTaskBuilder RunAll { get; set; }

        #endregion


        #region Recipes

        /// <summary>
        /// Gets a list of recipes configured to build
        /// </summary>
        public static List<Recipe> RecipeBuilds { get; } = new List<Recipe>();

        #endregion
    }
}
