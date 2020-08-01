using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Cake.Common.Solution.Project.Properties;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Common.Tools.NuGet.Pack;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using Cake.Core.IO;

using Higgsoft.Cake.ReleaseNotes;

namespace Higgsoft.Cake.Versions
{
    /// <summary>
    /// Cake extension methods for working with versions
    /// </summary>
    [CakeAliasCategory("Higgsoft.Version")]
    public static class VersionAliases
    {
        internal const string VersionLinePattern
            = @"^#+\s*(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)$";


        /// <summary>
        /// Parses the first lines of the supplied files for a version number and updates
        /// the version files in the supplied settings
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="pathToFile">Path to the file to be parsed</param>
        /// <param name="assemblyInfoSettings">Assembly info settings to be updated</param>
        /// <param name="dotNetCoreMSBuildSettings">DotNetCore build settings to be
        /// updated</param>
        /// <param name="nuGetPackSettings">NuGet pack settings to be updated</param>
        /// <param name="releaseNotesSettings">Release notes settings to be updated</param>
        /// <returns></returns>
        [CakeMethodAlias]
        public static Version ParseAndUpdateVersion(
            this ICakeContext context,
            FilePath pathToFile,
            AssemblyInfoSettings assemblyInfoSettings = null,
            DotNetCoreMSBuildSettings dotNetCoreMSBuildSettings = null,
            NuGetPackSettings nuGetPackSettings = null,
            ReleaseNotesSettings releaseNotesSettings = null)
        {
            var version = context.ParseVersionFrom(pathToFile);

            if (!(assemblyInfoSettings is null))
                context.UpdateAssemblyInfoVersion(version, assemblyInfoSettings);

            if (!(dotNetCoreMSBuildSettings is null))
                context.UpdateDotNetCoreBuildVersion(version, dotNetCoreMSBuildSettings);

            if (!(nuGetPackSettings is null))
                context.UpdateNuGetPackVersion(version, nuGetPackSettings);

            if (!(releaseNotesSettings is null))
                context.UpdateReleaseNotesVersion(version, releaseNotesSettings);

            return version;
        }


        /// <summary>
        /// Parses the first line of the supplied file for a version number
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="pathToFile">Path to the file to be parsed</param>
        /// <returns></returns>
        [CakeMethodAlias]
        public static Version ParseVersionFrom(
            this ICakeContext context,
            FilePath pathToFile)
        {
            if (pathToFile is null)
                throw new ArgumentNullException(nameof(pathToFile));

            var lines = File.ReadLines(pathToFile.FullPath);

            if (!lines.Any())
            {
                context.Log.Information($"No lines in {pathToFile}");
                return new Version(0, 0, 1);
            }

            var match = Regex.Match(lines.First(), VersionLinePattern);
            if (!match.Success)
            {
                context.Log.Information($"Unable to parse version from: {lines.First()}");
                return new Version(0, 0, 1);
            }

            return new Version(
                int.Parse(match.Groups["major"].Value),
                int.Parse(match.Groups["minor"].Value),
                int.Parse(match.Groups["patch"].Value));
        }


        /// <summary>
        /// Updates the version fields with the supplied version number
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="version">Version number to be set</param>
        /// <param name="settings">Setting to be updated</param>
        [CakeMethodAlias]
        public static void UpdateAssemblyInfoVersion(
            this ICakeContext context,
            Version version,
            AssemblyInfoSettings settings)
        {
            if (settings is null)
                throw new ArgumentNullException(nameof(settings));

            settings.Version = version.ToString();
            settings.FileVersion = version.ToString();
            settings.InformationalVersion = version.ToString();
        }


        /// <summary>
        /// Updates the version fields with the supplied version number
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="version">Version number to be set</param>
        /// <param name="settings">Setting to be updated</param>
        [CakeMethodAlias]
        public static void UpdateDotNetCoreBuildVersion(
            this ICakeContext context,
            Version verson,
            DotNetCoreMSBuildSettings settings)
        {
            if (settings is null)
                throw new ArgumentNullException(nameof(settings));

            settings.SetVersion(verson.ToString());
        }


        /// <summary>
        /// Updates the version fields with the supplied version number
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="version">Version number to be set</param>
        /// <param name="settings">Setting to be updated</param>
        [CakeMethodAlias]
        public static void UpdateNuGetPackVersion(
            this ICakeContext context,
            Version verson,
            NuGetPackSettings settings)
        {
            if (settings is null)
                throw new ArgumentNullException(nameof(settings));

            settings.Version = verson.ToString();
        }


        /// <summary>
        /// Updates the version fields with the supplied version number
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="version">Version number to be set</param>
        /// <param name="settings">Setting to be updated</param>
        [CakeMethodAlias]
        public static void UpdateReleaseNotesVersion(
            this ICakeContext context,
            Version verson,
            ReleaseNotesSettings settings)
        {
            if (settings is null)
                throw new ArgumentNullException(nameof(settings));

            settings.Version = verson;
        }
    }
}
