using System;
using Microsoft.Win32;

namespace KeyMapper.Classes
{
    public interface IRegistryTimestampService
    {
        DateTime GetRegistryKeyTimestamp(RegistryHive hive, string keyName);

        bool CanUserWriteToKey(RegistryHive hive, string keyName);
    }
}
