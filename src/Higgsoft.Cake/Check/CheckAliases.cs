using System;
using System.Collections.Generic;
using System.Text;

using Cake.Common.IO;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Git;

using Higgsoft.Cake.ReleaseNotes;

namespace Higgsoft.Cake.Check
{
    /// <summary>
    /// Delegate that takes in some settings to perform a pre-build check and return the
    /// results of that check
    /// </summary>
    /// <param name="context">Cake runtime context</param>
    /// <param name="checkSettings">Settings for running the pre-build checks</param>
    /// <returns></returns>
    public delegate ICheckResult CheckAction(
        ICakeContext context, CheckSettings checkSettings);


    /// <summary>
    /// Cake extension methods for performing pre-build checks
    /// </summary>
    [CakeAliasCategory("Higgsoft.Check")]
    public static class CheckAliases
    {
        /// <summary>
        /// Performs pre-build checks to ensure a consistent build state
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <example>
        /// <code>
        /// Task("check").Does(() => Check());
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Higgsoft.Checks")]
        public static void Check(this ICakeContext context)
            => context.Check(new CheckSettings());


        /// <summary>
        /// Performs pre-build checks to ensure a consistent build state
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="configSettings">Action to configure the settings for running
        /// the pre-build checks</param>
        /// <example>
        /// <code>
        /// Task("check")
        ///     .Does(() => {
        ///         Check(settings => {
        ///             settings.StagedChanges = false
        ///         });
        ///     });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Higgsoft.Checks")]
        public static void Check(
            this ICakeContext context,
            Action<CheckSettings> configSettings)
        {
            var settings = new CheckSettings();
            configSettings(settings);
            context.Check(settings);
        }


        /// <summary>
        /// Performs pre-build checks to ensure a consistent build state
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="checkSettings">Settings for running the pre-build checks</param>
        /// <example>
        /// <code>
        /// Task("check")
        ///     .Does(() => {
        ///         var settings = new CheckSettings();
        ///         Check(settings);
        ///     });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Higgsoft.Checks")]
        public static void Check(this ICakeContext context, CheckSettings checkSettings)
        {
            var anyErrors = false;
            var errorMessage = new StringBuilder();

            checkSettings.GitRoot = checkSettings.GitRoot
                ?? context.GitFindRootFromPath(
                    new DirectoryPath(".").MakeAbsolute(context.Environment));

            var checks = new List<CheckAction> {
                CheckStagedChanges,
                CheckUncommittedChanges,
                CheckUntrackedFiles,
                CheckReleaseNotesFileExists,
                CheckReleaseNotesVNextFileExists,
                CheckNewReleaseNotes
            };

            foreach (var check in checks)
            {
                var result = check(context, checkSettings);

                switch (result)
                {
                    case CheckSkipped _:
                        context.Log.Information($"Skipped: {result.Source}");
                        break;

                    case CheckPassed _:
                        context.Log.Information($"Passed: {result.Source}");
                        break;

                    case CheckFailed failure:
                        var message = $"Failed: {result.Source} - {failure.Message}";
                        context.Log.Error(message);
                        errorMessage.AppendLine(message);
                        anyErrors = true;
                        break;
                }
            }

            if (anyErrors)
                throw new InvalidOperationException(errorMessage.ToString());
        }


        /// <summary>
        /// Checks for staged files in the git repository
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="checkSettings">Settings for running the pre-build checks</param>
        /// <returns></returns>
        [CakeMethodAlias]
        [CakeAliasCategory("Higgsoft.Checks")]
        public static ICheckResult CheckStagedChanges(
            this ICakeContext context,
            CheckSettings checkSettings)
        {
            if (!checkSettings.StagedChanges)
                return new CheckSkipped();

            if (!context.GitHasStagedChanges(checkSettings.GitRoot))
                return new CheckPassed();

            return CheckFailed.WithReason(
                "Git has staged changed, please commit all files before building");
        }


        /// <summary>
        /// Checks for uncommitted changes in the git repository
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="checkSettings">Settings for running the pre-build checks</param>
        /// <returns></returns>
        [CakeMethodAlias]
        [CakeAliasCategory("Higgsoft.Checks")]
        public static ICheckResult CheckUncommittedChanges(
            this ICakeContext context,
            CheckSettings checkSettings)
        {
            if (!checkSettings.UncommittedChanges)
                return new CheckSkipped();

            if (!context.GitHasUncommitedChanges(checkSettings.GitRoot))
                return new CheckPassed();

            return CheckFailed.WithReason(
                "Git has uncommitted changes, please commit all files before building");
        }


        /// <summary>
        /// Checks for untracked files in the git repository
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="checkSettings">Settings for running the pre-build checks</param>
        /// <returns></returns>
        [CakeMethodAlias]
        [CakeAliasCategory("Higgsoft.Checks")]
        public static ICheckResult CheckUntrackedFiles(
            this ICakeContext context,
            CheckSettings checkSettings)
        {
            if (!checkSettings.UntrackedFiles)
                return new CheckSkipped();

            if (!context.GitHasUntrackedFiles(checkSettings.GitRoot))
                return new CheckPassed();

            return CheckFailed.WithReason(
                "Git has untracked files, please commit or ignore files before building");
        }


        /// <summary>
        /// Checks for the existence of the release notes file
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="checkSettings">Settings for running the pre-build checks</param>
        /// <returns></returns>
        [CakeMethodAlias]
        [CakeAliasCategory("Higgsoft.Checks")]
        public static ICheckResult CheckReleaseNotesFileExists(
            this ICakeContext context,
            CheckSettings checkSettings)
        {
            if (!checkSettings.RequireReleaseNotes)
                return new CheckSkipped();

            if (context.FileExists(checkSettings.ReleaseNotes))
                return new CheckPassed();

            return CheckFailed.WithReason(
                $"Unable to find release notes file: {checkSettings.ReleaseNotes}");
        }


        /// <summary>
        /// Checks for the existence of the release notes file
        /// </summary>
        /// <param name="context">Cake runtime context</param>
        /// <param name="checkSettings">Settings for running the pre-build checks</param>
        /// <returns></returns>
        [CakeMethodAlias]
        [CakeAliasCategory("Higgsoft.Checks")]
        public static ICheckResult CheckReleaseNotesVNextFileExists(
            this ICakeContext context,
            CheckSettings checkSettings)
        {
            if (!checkSettings.RequireReleaseNotes)
                return new CheckSkipped();

            if (context.FileExists(checkSettings.ReleaseNotesVNext))
                return new CheckPassed();

            return CheckFailed.WithReason(
                $"Unable to find release notes file: {checkSettings.ReleaseNotesVNext}");
        }


        /// <summary>
        /// Checks for lines in the release note v-next file
        /// </summary>
        /// <param name="context"></param>
        /// <param name="checkSettings"></param>
        /// <returns></returns>
        [CakeMethodAlias]
        [CakeAliasCategory("Higgsoft.Checks")]
        public static ICheckResult CheckNewReleaseNotes(
            this ICakeContext context,
            CheckSettings checkSettings)
        {
            if (!checkSettings.RequireReleaseNotes)
                return new CheckSkipped();

            if (context.AnyReleaseNotes(checkSettings.ReleaseNotesVNext))
                return new CheckPassed();

            return CheckFailed.WithReason(
                $"No new release notes found in {checkSettings.ReleaseNotesVNext}");
        }
    }
}
