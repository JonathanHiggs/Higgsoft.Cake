using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Cake.Common.IO;
using Cake.Common.Tools.NuGet.Pack;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

using Higgsoft.Cake.Versions;

namespace Higgsoft.Cake.ReleaseNotes
{
    /// <summary>
    /// Cake extension methods for working with release notes files
    /// </summary>
    [CakeAliasCategory("Higgsoft.ReleaseNotes")]
    public static class ReleaseNotesAliases
    {
        internal const string ReleaseNoteLinePattern = @"^[-*]\s*(.+)$";


        /// <summary>
        /// Checks to see whether there are any lines in the v-next release notes file
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="settings">Settings with the path to the release notes files</param>
        /// <returns>true if the there are any release notes in the v-next file; false
        /// otherwise</returns>
        /// <example>
        /// <code>
        /// Task("CheckReleaseNotes").Does(() => ReleaseNotesUpdated(settings)
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static bool ReleaseNotesUpdated(
            this ICakeContext context,
            ReleaseNotesSettings settings)
            => AnyReleaseNotes(context, settings.ReleaseNotesVNext);


        /// <summary>
        /// Prepends new release notes to the main release notes file
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="releaseNotesSettings">Settings with the path to the release notes files</param>
        /// <param name="nuGetPackSettings">NuGet pack settings to add release notes to</param>
        /// <returns>Collection of release notes with markdown formatting stripped out</returns>
        [CakeMethodAlias]
        public static ICollection<string> UpdateReleaseNotes(
            this ICakeContext context,
            ReleaseNotesSettings releaseNotesSettings,
            NuGetPackSettings nuGetPackSettings = null)
        {
            var releaseNotes =
                FilterLines(File.ReadLines(releaseNotesSettings.ReleaseNotesVNext.FullPath)).ToList();

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"## {releaseNotesSettings.Version}");
            stringBuilder.AppendLine();

            foreach (var line in releaseNotes)
                stringBuilder.AppendLine($"- {line}");

            stringBuilder.AppendLine();
            stringBuilder.Append(File.ReadAllText(releaseNotesSettings.ReleaseNotes.FullPath));
            File.WriteAllText(releaseNotesSettings.ReleaseNotes.FullPath, stringBuilder.ToString());

            var nextVersion = releaseNotesSettings.Version.Bump(BumpMethod.Patch);
            File.WriteAllText(releaseNotesSettings.ReleaseNotesVNext.FullPath, $"## {nextVersion}");

            if (!(nuGetPackSettings is null))
                nuGetPackSettings.ReleaseNotes = releaseNotes;

            return releaseNotes;
        }


        /// <summary>
        /// Returns a value that determines whether there are any release notes in the
        /// supplied file path
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="pathToFile">Path to the file</param>
        /// <returns>true if the file contains release notes; false otherwise</returns>
        [CakeMethodAlias]
        public static bool AnyReleaseNotes(this ICakeContext context, FilePath pathToFile)
        {
            if (!context.FileExists(pathToFile))
                return false;

            return FilterLines(File.ReadLines(pathToFile.FullPath)).Any();
        }


        /// <summary>
        /// Ensures that the release notes files exist at the supplied file paths
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="releaseNotes">Path to the release notes file</param>
        /// <param name="releaseNotesVNext">Path to the next-version release notes
        /// file</param>
        [CakeMethodAlias]
        public static void EnsureReleaseNotesExist(
            this ICakeContext context,
            FilePath releaseNotes,
            FilePath releaseNotesVNext)
        {
            if (!context.FileExists(releaseNotes))
                File.WriteAllText(releaseNotes.FullPath, "");

            if (!context.FileExists(releaseNotesVNext))
                File.WriteAllText(releaseNotesVNext.FullPath, "## 0.1.0");
        }


        /// <summary>
        /// Parses lines from a release notes file and filters version and blank lines
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static IEnumerable<string> FilterLines(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var match = Regex.Match(line, ReleaseNoteLinePattern);
                if (!match.Success)
                    continue;

                yield return match.Groups[1].Value;
            }
        }
    }
}
