using KeyMapper.Providers;
using NUnit.Framework;

namespace KeyMapper.UnitTests
{
    [TestFixture]
    public class OperatingSystemCapabilityProviderTests
    {
        [Test]
        public static void OS_Windows_2000_Is_Correct()
        {
            IOperatingSystemCapability provider = new OperatingSystemCapabilityProvider().CreateMockInstance(5, 0);

            Assert.AreEqual("Windows 2000", provider.OperatingSystem);
            Assert.IsFalse(provider.ImplementsTaskDialog);
            Assert.IsFalse(provider.ImplementsUAC);
            Assert.IsFalse(provider.SupportsUserMappings);
            Assert.IsFalse(provider.SupportsLocalizedKeyboardNames);
          }

        [Test]
        public static void OS_Windows_XP_Is_Correct()
        {
            var provider = new OperatingSystemCapabilityProvider().CreateMockInstance(5, 1);

            Assert.AreEqual("Windows XP", provider.OperatingSystem);
            Assert.IsFalse(provider.ImplementsTaskDialog);
            Assert.IsFalse(provider.ImplementsUAC);
            Assert.IsTrue(provider.SupportsUserMappings);
            Assert.IsTrue(provider.SupportsLocalizedKeyboardNames);
        }

        [Test]
        public static void OS_Windows_Vista_Is_Correct()
        {
            var provider = new OperatingSystemCapabilityProvider().CreateMockInstance(6, 0);
            Assert.AreEqual("Windows Vista", provider.OperatingSystem);
            Assert.IsTrue(provider.ImplementsTaskDialog);
            Assert.IsTrue(provider.ImplementsUAC);
            Assert.IsTrue(provider.SupportsUserMappings);
            Assert.IsTrue(provider.SupportsLocalizedKeyboardNames);
        }

        [Test]
        public static void OS_Windows_7_Is_Correct()
        {
            var provider = new OperatingSystemCapabilityProvider().CreateMockInstance(6, 1);
            Assert.AreEqual("Windows 7", provider.OperatingSystem);
            Assert.IsTrue(provider.ImplementsTaskDialog);
            Assert.IsTrue(provider.ImplementsUAC);
            Assert.IsFalse(provider.SupportsUserMappings);
            Assert.IsTrue(provider.SupportsLocalizedKeyboardNames);
        }

        [Test]
        public static void OS_Windows_8_Is_Correct()
        {
            var provider = new OperatingSystemCapabilityProvider().CreateMockInstance(6, 2);
            Assert.AreEqual("Windows 8", provider.OperatingSystem);
            Assert.IsTrue(provider.ImplementsTaskDialog);
            Assert.IsTrue(provider.ImplementsUAC);
            Assert.IsFalse(provider.SupportsUserMappings);
            Assert.IsTrue(provider.SupportsLocalizedKeyboardNames);
        }
    }
}
