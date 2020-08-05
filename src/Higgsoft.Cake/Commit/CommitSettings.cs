using System.Collections.Generic;

using Cake.Core.IO;

namespace Higgsoft.Cake.Commit
{
    /// <summary>
    /// Settings fr running post-build repository commit
    /// </summary>
    public class CommitSettings
    {
        /// <summary>
        /// Gets and sets a flag that indicates whether files changed during the build should be
        /// committed to the repository
        /// </summary>
        public bool CommitChanges { get; set; } = true;


        /// <summary>
        /// Gets and sets a flag that indicates whether a version tag is created
        /// </summary>
        public bool CreateVersionTag { get; set; } = true;


        /// <summary>
        /// Gets and sets a collection of files that will be committed
        /// </summary>
        public ICollection<FilePath> Files { get; set; } = null;


        /// <summary>
        /// Gets and sets the directory path to the git repository root
        /// </summary>
        public DirectoryPath GitRoot { get; set; } = null;


        /// <summary>
        /// Gets and sets the build version number
        /// </summary>
        public Versions.Version Version { get; set; }


        /// <summary>
        /// Gets and sets the build product name
        /// </summary>
        public string ProductName { get; set; } = string.Empty;


        /// <summary>
        /// Gets and sets the git username used when committing
        /// </summary>
        public string GitUserName { get; set; } = "CakeBuild";


        /// <summary>
        /// Gets and sets the git email used when committing
        /// </summary>
        public string GitEmail { get; set; } = "build@cake.com";
    }
}
