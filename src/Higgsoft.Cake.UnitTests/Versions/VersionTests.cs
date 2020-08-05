using System;

using NUnit.Framework;

namespace Higgsoft.Cake.UnitTests.Versions
{
    [TestFixture]
    public class VersionTests
    {
        [Test]
        public void TryParse_WithValidString_ReturnsExpectedVersion()
        {
            // Arrange
            var str = "1.2.3";
            var expected = new Version(1, 2, 3);

            // Act
            var result = Version.TryParse(str, out var version);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(version, Is.EqualTo(expected));
        }


        [Test]
        public void TryParse_WithInvalidString_ReturnsFalse()
        {
            // Arrange
            var str = "x.y.z";

            // Act
            var result = Version.TryParse(str, out var version);

            // Assert
            Assert.That(result, Is.False);
        }


        [Test]
        public void ToString_WithVersion_IsExpectedFormat()
        {
            // Arrange
            var version = new Version(1, 2, 3);
            var expected = "1.2.3";

            // Act
            var str = version.ToString();

            // Assert
            Assert.That(str, Is.EqualTo(expected));
        }


        [Test]
        public void Equals_WithMatchingVersion_IsTrue()
        {
            // Arrange
            var v1 = new Version("1.2.3");
            var v2 = new Version("1.2.3");

            // Act
            var result = v1 == v2;

            // Assert
            Assert.That(result, Is.True);
        }


        [Test]
        public void Equals_WithDifferentVersion_IsFalse()
        {
            // Arrange
            var v1 = new Version("1.2.3");
            var v2 = new Version("2.3.4");

            // Act
            var result = v1 == v2;

            // Assert
            Assert.That(result, Is.False);
        }


        [Test]
        public void GetHashCode_WithMatchingVersion_AreSame()
        {
            // Arrange
            var v1 = new Version("1.1.1");
            var v2 = new Version("1.1.1");

            // Act
            var code1 = v1.GetHashCode();
            var code2 = v2.GetHashCode();

            // Assert
            Assert.That(code1, Is.EqualTo(code2));
        }


        [Test]
        public void GetHashCode_WithDifferentVersion_AreDifferent()
        {
            // Arrange
            var v1 = new Version("1.2.3");
            var v2 = new Version("1.2.4");

            // Act
            var code1 = v1.GetHashCode();
            var code2 = v2.GetHashCode();

            // Assert
            Assert.That(code1, Is.Not.EqualTo(code2));
        }
    }
}
