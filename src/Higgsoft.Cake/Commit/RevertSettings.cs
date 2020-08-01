using System.Collections.Generic;

using Cake.Core.IO;

namespace Higgsoft.Cake.Commit
{
    /// <summary>
    /// Settings to control reverting changed files
    /// </summary>
    public class RevertSettings
    {
        /// <summary>
        /// Gets and sets the path to the git repository root
        /// </summary>
        public DirectoryPath GitRoot { get; set; }


        /// <summary>
        /// Gets and sets the <see cref="FilePath"/> to the assembly info file
        /// </summary>
        public FilePath AssemblyInfoFile { get; set; }


        /// <summary>
        /// Gets and sets the <see cref="FilePath"/> to the release notes file
        /// </summary>
        public FilePath ReleaseNotesFile { get; set; }


        /// <summary>
        /// Gets and sets the <see cref="FilePath"/> to the release notes v-next file
        /// </summary>
        public FilePath ReleaseNotesVNextFile { get; set; }


        /// <summary>
        /// Gets and sets a sequence of files to be included in the revert
        /// </summary>
        public IEnumerable<FilePath> Files { get; set; } = null;
    }
}
