using Cake.Core.IO;

namespace Higgsoft.Cake.Check
{
    /// <summary>
    /// Setting for running pre-build checks
    /// </summary>
    public class CheckSettings
    {
        /// <summary>
        /// Gets and sets the path to the git repository root directory
        /// </summary>
        public DirectoryPath GitRoot { get; set; }


        /// <summary>
        /// Gets and sets a value that determines whether to check for staged files
        /// </summary>
        public bool StagedChanges { get; set; } = true;


        /// <summary>
        /// Gets and sets a value that determines whether to check for uncommitted files
        /// </summary>
        public bool UncommittedChanges { get; set; } = true;


        /// <summary>
        /// Gets and sets a value that determines whether to check for untracked files
        /// </summary>
        public bool UntrackedFiles { get; set; } = true;


        /// <summary>
        /// Gets and sets a value that determines whether to check for a release notes
        /// file
        /// </summary>
        public bool RequireReleaseNotes { get; set; } = true;


        /// <summary>
        /// Gets and sets the path to the release notes file
        /// </summary>
        public FilePath ReleaseNotesFile { get; set; }
            = new FilePath("./ReleaseNotes.md");


        /// <summary>
        /// Gets and sets the path to the next version release notes file
        /// </summary>
        public FilePath ReleaseNotesVNextFile { get; set; }
            = new FilePath("./ReleaseNotes.vnext.md");


        // ToDo: list of additional check methods
    }
}
