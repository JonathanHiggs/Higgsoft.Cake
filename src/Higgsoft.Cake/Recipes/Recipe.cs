using System;
using System.Collections.Generic;

using Cake.Common.Solution.Project.Properties;
using Cake.Common.Tools.NUnit;
using Cake.Core;
using Cake.Core.IO;

using Higgsoft.Cake.Check;
using Higgsoft.Cake.Commit;
using Higgsoft.Cake.ReleaseNotes;

namespace Higgsoft.Cake.Recipes
{
    /// <summary>
    /// Shared recipe properties
    /// </summary>
    public abstract class Recipe
    {
        #region General

        private string id;


        /// <summary>
        /// Gets and sets the recipe identity
        /// </summary>
        public string Id
        {
            get => id;
            set
            {
                id = value;
                OnIdChanged(id);
            }
        }


        /// <summary>
        /// Gets and sets a human readable name
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Gets and sets the description
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// Gets and sets the solution name
        /// </summary>
        public string Solution { get; set; }


        /// <summary>
        /// Gets and sets the project name
        /// </summary>
        public string Project { get; set; }


        /// <summary>
        /// Gets and sets the project GUID used in the assembly info file
        /// </summary>
        public string Guid { get; set; }


        /// <summary>
        /// Gets and sets the version
        /// </summary>
        public Versions.Version Version { get; set; }


        /// <summary>
        /// Get and sets a flat that determines whether the assembly info file is shared between
        /// multiple projects
        /// </summary>
        public bool SharedAssemblyInfoFile { get; set; } = false;


        /// <summary>
        /// Gets and sets a list of release notes
        /// </summary>
        public List<string> ReleaseNotes { get; set; } = new List<string>();

        #endregion


        #region TaskControl

        /// <summary>
        /// Gets and sets a flag that determines whether to include the pre-build task
        /// </summary>
        public bool UsePreBuildTask { get; set; } = false;


        /// <summary>
        /// Gets and sets a flag that determines whether to include the post-build task
        /// </summary>
        public bool UsePostBuildTask { get; set; } = false;

        #endregion


        #region FlowControl

        /// <summary>
        /// Gets and sets a flag that determines whether release notes are updated during the build
        /// </summary>
        public bool PrepareReleaseNotes { get; set; } = true;


        /// <summary>
        /// Gets and sets a flag that determines whether the assembly info file is updated during
        /// the build
        /// </summary>
        public bool UpdateAssemblyInfo { get; set; } = true;


        /// <summary>
        /// Gets a flag that determines whether any changes made during the build are committed
        /// </summary>
        public bool UseCommitTask => !Build.Local && Build.EnableCommits && CommitChanges;


        /// <summary>
        /// Gets and sets a flag that aborts the remaining tasks in the build pipeline once toggled
        /// </summary>
        public bool SkipRemainingTasks { get; set; } = false;


        /// <summary>
        /// Gets a flag that indicates whether the recipe build failed
        /// </summary>
        public bool Errored { get; private set; } = false;


        /// <summary>
        /// Gets the name of the task that failed
        /// </summary>
        public string ErroredTaskName { get; private set; } = string.Empty;


        /// <summary>
        /// Gets the exception that was thrown during the build
        /// </summary>
        public Exception Exception { get; private set; }


        /// <summary>
        /// Sets the recipe error state
        /// </summary>
        /// <param name="task">Task that throw an exception</param>
        /// <param name="ex">Exception thrown</param>
        public void SetError(CakeTaskBuilder task, Exception ex)
            => SetError(task.Task.Name, ex);


        /// <summary>
        /// Sets the recipe error state
        /// </summary>
        /// <param name="taskName">Name of the task that throw an exception</param>
        /// <param name="ex">Exception thrown</param>
        public void SetError(string taskName, Exception ex)
        {
            Errored = true;
            ErroredTaskName = taskName;
            Exception = ex;
        }

        #endregion


        #region Paths

        /// <summary>
        /// Gets and sets the path to the solution directory
        /// </summary>
        public DirectoryPath SolutionDirectory { get; set; }


        /// <summary>
        /// Gets and sets the path to the solution file
        /// </summary>
        public FilePath SolutionFile { get; set; }


        /// <summary>
        /// Gets and sets the path to the project file
        /// </summary>
        public FilePath ProjectFile { get; set; }


        /// <summary>
        /// Gets and sets the path to the assembly info file
        /// </summary>
        public FilePath AssemblyInfoFile { get; set; }


        /// <summary>
        /// Gets and sets the path to the release notes file
        /// </summary>
        public FilePath ReleaseNotesFile { get; set; }


        /// <summary>
        /// Gets and sets the path to the release notes v-next files
        /// </summary>
        public FilePath ReleaseNotesVNextFile { get; set; }

        #endregion


        #region CommitSettings

        /// <summary>
        /// Gets and sets a flag that determines whether file changes are committed to the git
        /// repository
        /// </summary>
        public bool CommitChanges { get; set; } = true;


        /// <summary>
        /// Gets and sets a flag that determines whether a git version tag is created
        /// </summary>
        public bool TagVersion { get; set; } = true;

        #endregion


        #region Generated

        /// <summary>
        /// Gets the <see cref="Check.CheckSettings"/> for the recipe
        /// </summary>
        public CheckSettings CheckSettings
            => new CheckSettings {
                GitRoot = Build.GitRoot,
                ReleaseNotesFile = ReleaseNotesFile,
                ReleaseNotesVNextFile = ReleaseNotesVNextFile,
                RequireReleaseNotes = PrepareReleaseNotes,
                // These are checked at the build level
                StagedChanges = false,
                UncommittedChanges = false,
                UntrackedFiles = false
            };


        /// <summary>
        /// Gets the <see cref="ReleaseNotes.ReleaseNotesSettings"/> for the recipe
        /// </summary>
        public ReleaseNotesSettings ReleaseNotesSettings
            => new ReleaseNotesSettings {
                ReleaseNotes = ReleaseNotesFile,
                ReleaseNotesVNext = ReleaseNotesVNextFile,
                Version = Version
            };


        /// <summary>
        /// Gets the <see cref="global::Cake.Common.Solution.Project.Properties.AssemblyInfoSettings"/>
        /// for the recipe
        /// </summary>
        public AssemblyInfoSettings AssemblyInfoSettings
            => new AssemblyInfoSettings {
                Title = !SharedAssemblyInfoFile ? Project : string.Empty,
                Guid = !SharedAssemblyInfoFile ? Guid : string.Empty,
                Description = Description,
                Product = Solution,
                Company = Build.Company,
                Copyright = $"Copyright (c) {Build.Company} {DateTime.Today.Year}",
                Configuration = Build.Configuration,
                Version = Version.ToString(),
                FileVersion = Version.ToString(),
                InformationalVersion = Version.ToString(),
                ComVisible = false
            };


        /// <summary>
        /// Gets the <see cref="NUnit3Settings"/> for the recipe
        /// </summary>
        public NUnit3Settings NUnitSettings
            => new NUnit3Settings { NoResults = true };


        /// <summary>
        /// Gets the <see cref="Commit.CommitSettings"/> for the recipe
        /// </summary>
        public CommitSettings CommitSettings
            => new CommitSettings {
                CommitChanges = Build.EnableCommits && CommitChanges,
                CreateVersionTag = Build.EnableTags && CommitChanges && TagVersion,
                GitEmail = Build.GitEmail,
                GitRoot = Build.GitRoot,
                GitUserName = Build.GitUserName,
                ProductName = Name,
                Version = Version
            };


        /// <summary>
        /// Gets the <see cref="Commit.RevertSettings"/> for the recipe
        /// </summary>
        public RevertSettings RevertSettings
            => new RevertSettings {
                GitRoot = Build.GitRoot,
                AssemblyInfoFile = AssemblyInfoFile,
                ReleaseNotesFile = ReleaseNotesFile,
                ReleaseNotesVNextFile = ReleaseNotesVNextFile
            };

        #endregion


        /// <summary>
        /// Returns a string representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => Name;


        /// <summary>
        /// Coroutine called when <see cref="Id"/> value is set
        /// </summary>
        /// <param name="id"></param>
        protected abstract void OnIdChanged(string id);
    }
}
