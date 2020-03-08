using System.Runtime.CompilerServices;

namespace Higgsoft.Cake.Check
{
    /// <summary>
    /// Represents a failing check
    /// </summary>
    public class CheckFailed : ICheckResult
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CheckFailed"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public CheckFailed(string source, string message)
        {
            Message = message ?? string.Empty;
            Source = source ?? string.Empty;
        }


        /// <summary>
        /// Returns a new instance of <see cref="CheckFailed"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static CheckFailed WithReason(
            string message,
            [CallerMemberName] string source = "")
            => new CheckFailed(source, message);


        /// <summary>
        /// Gets the error message
        /// </summary>
        public string Message { get; }


        /// <summary>
        /// Gets the error source
        /// </summary>
        public string Source { get; }


        /// <summary>
        /// Gets a value that determines whether the check failed
        /// </summary>
        public bool Failed => true;


        /// <summary>
        /// Returns a string representation of the error
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{Source} - {Message}";
    }
}
