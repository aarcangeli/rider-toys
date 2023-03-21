using JetBrains.Application;
using JetBrains.Util;

namespace ReSharperPlugin.RiderTools;

[ShellComponent]
public class TestComponent
{
    private readonly ILogger _logger;

    public TestComponent(ILogger logger)
    {
        _logger = logger;
        _logger.Info("Initializing Test Component");
    }
}
