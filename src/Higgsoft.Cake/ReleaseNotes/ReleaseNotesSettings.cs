using Cake.Core.IO;

using Higgsoft.Cake.Versions;

namespace Higgsoft.Cake.ReleaseNotes
{
    /// <summary>
    /// Settings for controlling release notes tasks
    /// </summary>
    public class ReleaseNotesSettings
    {
        /// <summary>
        /// Gets and sets the path to the release notes file
        /// </summary>
        public FilePath ReleaseNotes { get; set; } = new FilePath("./ReleaseNotes.md");


        /// <summary>
        /// Gets and sets the path to the v-next release notes file
        /// </summary>
        public FilePath ReleaseNotesVNext { get; set; } = new FilePath("./ReleaseNotes.vnext.md");


        public Version Version { get; set; }
    }
}