using System;
using System.Text.RegularExpressions;

namespace Higgsoft.Cake.Versions
{
    /// <summary>
    /// Represents an assembly version, simplification of semantic versioning
    /// </summary>
    public class Version
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Version"/>
        /// </summary>
        /// <param name="major">Major version number</param>
        /// <param name="minor">Minor version number</param>
        /// <param name="patch">Patch version number</param>
        public Version(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }


        /// <summary>
        /// Gets and the major version number
        /// </summary>
        public int Major { get; }


        /// <summary>
        /// Gets the minor version number
        /// </summary>
        public int Minor { get; set; }


        /// <summary>
        /// Gets the patch version number
        /// </summary>
        public int Patch { get; set; }


        /// <summary>
        /// Attempts to parse a version from the supplied string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="version"></param>
        /// <returns>true if a version is successfully parsed; false otherwise</returns>
        public static bool TryParse(string str, out Version version)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                version = null;
                return false;
            }

            const string pattern = @"(\d+)\.(\d+)\.(\d+)$";

            var match = Regex.Match(str, pattern);

            if (!match.Success)
            {
                version = null;
                return false;
            }

            version = new Version(
                int.Parse(match.Groups[1].Value),
                int.Parse(match.Groups[2].Value),
                int.Parse(match.Groups[3].Value));

            return true;
        }


        /// <summary>
        /// Parses a version from the supplied string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Version Parse(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentNullException(nameof(str));

            return TryParse(str, out var version)
                ? version
                : throw new ArgumentException($"Unable to parse version from: {str}");
        }


        /// <summary>
        /// Returns a string representation of the version
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"{Major}.{Minor}.{Patch}";


        /// <summary>
        /// Returns a value that determines whether the supplied object is equal to this
        /// instance
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
            => obj is Version other
                && Major == other.Major
                && Minor == other.Minor
                && Patch == other.Patch;


        /// <summary>
        /// Returns the hash code for this instance
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = -639545495;
                hashCode = hashCode * -1521134295 + Major.GetHashCode();
                hashCode = hashCode * -1521134295 + Minor.GetHashCode();
                hashCode = hashCode * -1521134295 + Patch.GetHashCode();
                return hashCode;
            }
        }


        /// <summary>
        /// Equality operator
        /// </summary>
        /// <param name="lhs">Left operand</param>
        /// <param name="rhs">Right operand</param>
        /// <returns></returns>
        public static bool operator ==(Version lhs, Version rhs)
            => !(lhs is null)
                && !(rhs is null)
                && lhs.Major == rhs.Major
                && lhs.Minor == rhs.Minor
                && lhs.Patch == rhs.Patch;


        /// <summary>
        /// Inequality operator
        /// </summary>
        /// <param name="lhs">Left operand</param>
        /// <param name="rhs">Right operand</param>
        /// <returns></returns>
        public static bool operator !=(Version lhs, Version rhs)
            => !(lhs == rhs);


        /// <summary>
        /// Greater-than operator
        /// </summary>
        /// <param name="lhs">Left operand</param>
        /// <param name="rhs">Right operand</param>
        /// <returns></returns>
        public static bool operator >(Version lhs, Version rhs)
            => lhs.Major > rhs.Major
                || (lhs.Major == rhs.Major && lhs.Minor > rhs.Minor)
                || (lhs.Major == rhs.Major && lhs.Minor == rhs.Minor && lhs.Patch > rhs.Patch);


        /// <summary>
        /// Less-than operator
        /// </summary>
        /// <param name="lhs">Left operand</param>
        /// <param name="rhs">Right operand</param>
        /// <returns></returns>
        public static bool operator <(Version lhs, Version rhs)
            => lhs.Major < rhs.Major
                || (lhs.Major == rhs.Major && lhs.Minor < rhs.Minor)
                || (lhs.Major == rhs.Major && lhs.Minor == rhs.Minor && lhs.Patch < rhs.Patch);


        /// <summary>
        /// Returns a version with an incremented version number
        /// </summary>
        /// <param name="bump"></param>
        /// <returns></returns>
        public Version Bump(BumpMethod bump)
        {
#if NETCOREAPP3_1
            return bump switch
            {
                BumpMethod.Major => new Version(Major + 1, 0, 0),
                BumpMethod.Minor => new Version(Major, Minor + 1, 0),
                BumpMethod.Patch => new Version(Major, Minor, Patch + 1),
                _ => throw new NotImplementedException()
            };
#else
            if (bump == BumpMethod.Major)
                return new Version(Major + 1, 0, 0);

            if (bump == BumpMethod.Minor)
                return new Version(Major, Minor + 1, 0);

            if (bump == BumpMethod.Patch)
                return new Version(Major, Minor, Patch + 1);

            throw new NotImplementedException();
#endif
        }
    }
}
