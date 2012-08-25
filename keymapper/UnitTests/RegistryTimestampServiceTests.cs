using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyMapper.Classes;
using Microsoft.Win32;
using NUnit.Framework;

namespace KeyMapper.UnitTests
{
    [TestFixture]
    internal class RegistryTimestampServiceTests
    {
        private IRegistryTimestampService registryTimestampService;

        [SetUp]
        public void Setup()
        {
            this.registryTimestampService = new RegistryTimestampService();
        }

        [Test]
        public void Can_Access_All_Hives()
        {
            // All standard Windows keys. If any of these are missing then it's Weird City.
            // One per hive.
            DateTime dt = this.registryTimestampService.GetRegistryKeyTimestamp(RegistryHive.ClassesRoot, "*");

            dt = this.registryTimestampService.GetRegistryKeyTimestamp(RegistryHive.CurrentUser, "Console");
            Assert.AreNotEqual(DateTime.MinValue, dt);

            dt = this.registryTimestampService.GetRegistryKeyTimestamp(RegistryHive.LocalMachine, "SAM");
            Assert.AreNotEqual(DateTime.MinValue, dt);

            dt = this.registryTimestampService.GetRegistryKeyTimestamp(RegistryHive.Users, ".DEFAULT");
            Assert.AreNotEqual(DateTime.MinValue, dt);

            dt = this.registryTimestampService.GetRegistryKeyTimestamp(RegistryHive.CurrentConfig, "Software");
            Assert.AreNotEqual(DateTime.MinValue, dt);
        }

        [Test]
        public void Non_Existing_Keys_Return_Minvalue()
        {
            // None of these keys will exist
            DateTime dt = this.registryTimestampService.GetRegistryKeyTimestamp(RegistryHive.DynData, "foo_bar_baz");
            Assert.AreEqual(DateTime.MinValue, dt);

            dt = this.registryTimestampService.GetRegistryKeyTimestamp(RegistryHive.CurrentUser, "Noway_key_exists");
            Assert.AreEqual(DateTime.MinValue, dt);

            dt = this.registryTimestampService.GetRegistryKeyTimestamp(RegistryHive.CurrentUser, null);
            Assert.AreEqual(DateTime.MinValue, dt);
        }

        [Test]
        public void Get_Time_From_Last_Login()
        {
            DateTime dt = this.registryTimestampService.GetRegistryKeyTimestamp(RegistryHive.CurrentUser, "Volatile Environment");
            Assert.AreNotEqual(DateTime.MinValue, dt);
            // Console.WriteLine("Logontime: {0}, Now: {1}, Time since last logon {2}", dt, DateTime.UtcNow, DateTime.UtcNow - dt);
        }
    }
}
