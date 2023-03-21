using JetBrains.ProjectModel;
using JetBrains.Util;

namespace ReSharperPlugin.RiderTools;

[SolutionComponent]
public class TestSolutionComponent
{
    private readonly ILogger _logger;

    public TestSolutionComponent(ILogger logger)
    {
        _logger = logger;
        _logger.Info("Initializing Test Component");
    }
}
