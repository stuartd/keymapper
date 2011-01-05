using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyMapper.Providers;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class OperatingSystemVersionProviderTests
    {
        [Test]
        public static void OS_Windows_2000_Is_Correct()
        {
            OperatingSystemVersionProvider.Mock(5, 0);
            Assert.AreEqual("Windows 2000", OperatingSystemVersionProvider.OperatingSystem);
            Assert.IsFalse(OperatingSystemVersionProvider.OperatingSystemImplementsTaskDialog);
            Assert.IsFalse(OperatingSystemVersionProvider.OperatingSystemImplementsUAC);
            Assert.IsFalse(OperatingSystemVersionProvider.OperatingSystemSupportsUserMappings);
        }

        [Test]
        public static void OS_Windows_XP_Is_Correct()
        {
            OperatingSystemVersionProvider.Mock(5, 1);
            Assert.AreEqual("Windows XP", OperatingSystemVersionProvider.OperatingSystem);
            Assert.IsFalse(OperatingSystemVersionProvider.OperatingSystemImplementsTaskDialog);
            Assert.IsFalse(OperatingSystemVersionProvider.OperatingSystemImplementsUAC);
            Assert.IsTrue(OperatingSystemVersionProvider.OperatingSystemSupportsUserMappings);
        }

        [Test]
        public static void OS_Windows_Vista_Is_Correct()
        {
            OperatingSystemVersionProvider.Mock(6, 0);
            Assert.AreEqual("Windows Vista", OperatingSystemVersionProvider.OperatingSystem);
            Assert.IsTrue(OperatingSystemVersionProvider.OperatingSystemImplementsTaskDialog);
            Assert.IsTrue(OperatingSystemVersionProvider.OperatingSystemImplementsUAC);
            Assert.IsTrue(OperatingSystemVersionProvider.OperatingSystemSupportsUserMappings);
        }

        [Test]
        public static void OS_Windows_7_Is_Correct()
        {
            OperatingSystemVersionProvider.Mock(6, 1);
            Assert.AreEqual("Windows 7", OperatingSystemVersionProvider.OperatingSystem);
            Assert.IsTrue(OperatingSystemVersionProvider.OperatingSystemImplementsTaskDialog);
            Assert.IsTrue(OperatingSystemVersionProvider.OperatingSystemImplementsUAC);
            Assert.IsFalse(OperatingSystemVersionProvider.OperatingSystemSupportsUserMappings);
        }

        [Test]
        public static void OS_Hypothetical_Windows_8_Behaves_Like_Windows_7()
        {
            OperatingSystemVersionProvider.Mock(7, 0);
            Assert.AreEqual("Windows 7", OperatingSystemVersionProvider.OperatingSystem);
            Assert.IsTrue(OperatingSystemVersionProvider.OperatingSystemImplementsTaskDialog);
            Assert.IsTrue(OperatingSystemVersionProvider.OperatingSystemImplementsUAC);
            Assert.IsFalse(OperatingSystemVersionProvider.OperatingSystemSupportsUserMappings);
        }
    }
}
