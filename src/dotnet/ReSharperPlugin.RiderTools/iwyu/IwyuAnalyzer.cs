using System;
using System.Linq;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Cpp.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Cpp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharperPlugin.RiderTools.iwyu;

[ElementProblemAnalyzer(ElementTypes: new[] { typeof(CppFile) },
    HighlightingTypes = new[] { typeof(RecomputeIncludesHighlighting) })]
public class IwyuAnalyzer : ICppSlowElementProblemAnalyzer
{
    public void Run(ITreeNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        var file = element as CppFile ?? throw new InvalidOperationException();
        var includes = IwyuIncludeUtils.FindAllIncludesPaths(file).ToSet();

        UsedIncludesProcessor.ProcessUsedIncludes(file, (node, location) =>
        {
            if (!includes.Contains(location))
            {
                consumer.AddHighlighting(new IwyuSymbolNotImported(node, location));
            }
        });
    }
}
