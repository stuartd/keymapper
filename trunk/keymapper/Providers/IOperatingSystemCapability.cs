using System;
namespace KeyMapper.Providers
{
    public interface IOperatingSystemCapability
    {
        string OperatingSystem { get; }
        bool ImplementsTaskDialog { get; }
        bool ImplementsUAC { get; }
        bool SupportsUserMappings { get; }
        bool SupportsLocalizedKeyboardNames { get; }
    }
}
