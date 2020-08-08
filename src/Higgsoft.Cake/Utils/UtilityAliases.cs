using System;

using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

namespace Higgsoft.Cake.Utils
{
    /// <summary>
    /// Utility methods for installing nuget packages during cake script execution
    /// </summary>
    [CakeAliasCategory("Higgsoft.Cake.Utils")]
    public static class UtilityAliases
    {
        /// <summary>
        /// Installs a NuGet package into the cake tools directory
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="settings">Settings to control the install</param>
        [CakeMethodAlias]
        public static void CakeInstallPackage(
            this ICakeContext context,
            Action<CakeInstallPackageSettings> settings)
        {
            var installSettings = new CakeInstallPackageSettings
            {
                NugetDirectory = context.Directory("./nuget"),
                ToolsDirectory = context.Directory("./tools")
            };

            settings(installSettings);
            context.CakeInstallPackage(installSettings);
        }


        /// <summary>
        /// Installs a NuGet package into the cake tools directory
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="settings">Settings to control the install</param>
        [CakeMethodAlias]
        public static void CakeInstallPackage(
            this ICakeContext context,
            CakeInstallPackageSettings settings)
        {
            if (string.IsNullOrEmpty(settings.Id))
                throw new ArgumentNullException("Id is empty");

            if (string.IsNullOrEmpty(settings.Version))
                throw new ArgumentNullException("Version is empty");

            if (settings.NugetDirectory?.FullPath is null
                || string.IsNullOrEmpty(settings.NugetDirectory.FullPath))
                throw new ArgumentNullException("NuGet package path is empty");

            if (settings.ToolsDirectory?.FullPath is null
                || string.IsNullOrEmpty(settings.ToolsDirectory.FullPath))
                throw new ArgumentNullException("Tools directory path is empty");

            if (!settings.AsAddin && !settings.AsTool)
                throw new ArgumentException("Not installing as addin or tool, call is redundent");

            var nugetPackage = context.File(
                $"{settings.NugetDirectory}/{settings.Id}.{settings.Version}.nupkg");

            if (settings.AsAddin)
            {
                var addinsDirectory = context.Directory(
                    $"{settings.ToolsDirectory.FullPath}/Addins/{settings.Id}.{settings.Version}");

                CakeInstallPackage(context, addinsDirectory, nugetPackage);
            }

            if (settings.AsTool)
            {
                var toolsDirectory = context.Directory(
                    $"{settings.ToolsDirectory.FullPath}/{settings.Id}.{settings.Version}");

                CakeInstallPackage(context, toolsDirectory, nugetPackage);
            }
        }


        /// <summary>
        /// Installs a NuGet package into the specified directory
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="targetDirectory">Target directory path</param>
        /// <param name="nugetPackage">NuGet package path</param>
        public static void CakeInstallPackage(
            ICakeContext context, 
            DirectoryPath targetDirectory,
            FilePath nugetPackage)
        {
            var deleteSettings = new DeleteDirectorySettings { Force = true, Recursive = true };

            if (context.DirectoryExists(targetDirectory))
            {
                context.Information($"Cleaning: {targetDirectory}");
                context.DeleteDirectory(targetDirectory, deleteSettings);
            }

            context.Information($"Installing {nugetPackage} to {targetDirectory}");

            context.Unzip(nugetPackage, targetDirectory);

            context.CopyFile(
                nugetPackage,
                context.File($"{targetDirectory}/{System.IO.Path.GetFileName(nugetPackage.FullPath)}"));
        }


        /// <summary>
        /// Removes an installed NuGet package installed by cake
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="settings">Settings to control the clean</param>
        [CakeMethodAlias]
        public static void CakeCleanPackage(
            this ICakeContext context,
            Action<CakeCleanPackageSettings> settings)
        {
            var cleanSettings = new CakeCleanPackageSettings
            {
                ToolsDirectory = context.Directory("./tools")
            };

            settings(cleanSettings);

            context.CakeCleanPackage(cleanSettings);
        }


        /// <summary>
        /// Removes an installed NuGet package installed by cake
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="settings">Settings to control the clean</param>
        [CakeMethodAlias]
        public static void CakeCleanPackage(
            this ICakeContext context,
            CakeCleanPackageSettings settings)
        {
            if (string.IsNullOrEmpty(settings.Id))
                throw new ArgumentNullException("Id is empty");

            if (string.IsNullOrEmpty(settings.Version))
                throw new ArgumentNullException("Version is empty");

            if (settings.ToolsDirectory?.FullPath is null
                || string.IsNullOrEmpty(settings.ToolsDirectory.FullPath))
                throw new ArgumentNullException("Tools directory path is empty");

            var deleteSettings = new DeleteDirectorySettings { Force = true, Recursive = true };

            var addinDirectory = context.Directory(
                $"{settings.ToolsDirectory}/Addins/{settings.Id}.{settings.Version}");

            if (context.DirectoryExists(addinDirectory))
            {
                context.Information($"Removing {addinDirectory}");
                context.DeleteDirectory(addinDirectory, deleteSettings);
            }

            var toolsDirectory = context.Directory(
                $"{settings.ToolsDirectory}/{settings.Id}.{settings.Version}");

            if (context.DirectoryExists(toolsDirectory))
            {
                context.Information($"Removing {toolsDirectory}");
                context.DeleteDirectory(toolsDirectory, deleteSettings);
            }
        }
    }
}
