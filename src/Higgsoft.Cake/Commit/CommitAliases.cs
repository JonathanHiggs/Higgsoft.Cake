using System;
using System.Linq;

using Cake.Common;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.Git;

namespace Higgsoft.Cake.Commit
{
    /// <summary>
    /// Cake extension methods for working with build commits
    /// </summary>
    [CakeAliasCategory("Higgsoft.Commit")]
    public static class CommitAliases
    {
        /// <summary>
        /// Commits and pushes changes to files made during the build process
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="commitSettings">Settings to control the commit process</param>
        /// <example>
        /// <code>
        /// Task("Commit").Does(() => CommitChanges(new CommitSettings()));
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void CommitChanges(this ICakeContext context, CommitSettings commitSettings)
        {
            if (!commitSettings.CommitChanges)
                return;

            if (commitSettings.Version is null)
                throw new ArgumentException("CommitSettings requires the Version to be set");

            if (commitSettings.GitRoot is null)
                commitSettings.GitRoot
                    = context.GitFindRootFromPath(
                        new DirectoryPath(".").MakeAbsolute(context.Environment));

            if (commitSettings.Files is null || !commitSettings.Files.Any())
                context.GitAddAll(commitSettings.GitRoot);
            else
                context.GitAdd(commitSettings.GitRoot, commitSettings.Files.ToArray());

            var commitMessage =
                !string.IsNullOrEmpty(commitSettings.ProductName)
                ? $"{commitSettings.ProductName} Version - {commitSettings.Version}"
                : $"Version - {commitSettings.Version}";

            context.GitCommit(
                commitSettings.GitRoot,
                commitSettings.GitUserName,
                commitSettings.GitEmail,
                commitMessage);

            if (commitSettings.CreateVersionTag)
                context.GitTag(commitSettings.GitRoot, $"v{commitSettings.Version}");

            if (commitSettings.PushToRemote)
            {
                context.StartProcess(
                    "git",
                    new ProcessSettings { Arguments = $"push {commitSettings.GitRemote}" });

                context.StartProcess(
                    "git",
                    new ProcessSettings { Arguments = $"push {commitSettings.GitRemote} --tags" });
            }
        }


        /// <summary>
        /// Reverts and changes to files made during the build
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="revertSettings">Settings to control the revert</param>
        /// <example>
        /// <code>
        /// Task("Revert").Does(() => RevertChanges(new RevertSettings()));
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void RevertChanges(this ICakeContext context, RevertSettings revertSettings)
        {
            var files = new[] {
                revertSettings.AssemblyInfoFile,
                revertSettings.ReleaseNotesFile,
                revertSettings.ReleaseNotesVNextFile
            }.Where(f => !(f is null));

            if (!(revertSettings.Files is null))
                files.Concat(revertSettings.Files);

            context.GitCheckout(
                revertSettings.GitRoot,
                files.ToArray());
        }
    }
}
