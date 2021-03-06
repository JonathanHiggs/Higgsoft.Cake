﻿using System;
using System.Collections.Generic;

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
        public static string Target { get; set; } = "Build-RunAll";


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


        /// <summary>
        /// Gets the build tasks
        /// </summary>
        public static BuildTasks Tasks { get; } = new BuildTasks();


        /// <summary>
        /// Gets the build targets
        /// </summary>
        public static BuildTargets Targets { get; } = new BuildTargets();

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


        /// <summary>
        /// Gets the <see cref="Check.CheckSettings"/> for the build
        /// </summary>
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
        /// Gets and sets the name of the git remote
        /// </summary>
        public static string GitRemoteName { get; set; } = "origin";


        /// <summary>
        /// Gets and sets a flag that enables git repository commits
        /// </summary>
        public static bool EnableCommits { get; set; } = true;


        /// <summary>
        /// Gets and sets a flag that enables git repository version tags
        /// </summary>
        public static bool EnableTags { get; set; } = true;


        /// <summary>
        /// Gets and sets a flag that enables the git remote push of commits
        /// </summary>
        public static bool EnablePush { get; set; } = true;


        /// <summary>
        /// Gets a flag that determines whether to push commits to the remote
        /// </summary>
        public static bool ShouldPush
            => EnableCommits && EnablePush && !Local;

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


        #region Artefacts

        /// <summary>
        /// Gets and sets the path to the artefacts repository
        /// </summary>
        public static DirectoryPath ArtefactsRepository { get; set; }


        /// <summary>
        /// Gets and sets the path to the local artefacts repository
        /// </summary>
        public static DirectoryPath ArtefactsLocalRepository { get; set; }
            = new DirectoryPath("./artefacts");

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


        #region Recipes

        /// <summary>
        /// Gets a list of recipes configured to build
        /// </summary>
        public static List<Recipe> RecipeBuilds { get; } = new List<Recipe>();

        #endregion
    }
}
