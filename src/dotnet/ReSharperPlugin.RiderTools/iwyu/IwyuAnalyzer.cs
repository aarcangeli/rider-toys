using System;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Cpp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharperPlugin.RiderTools.iwyu;

[ElementProblemAnalyzer(ElementTypes: new[] { typeof(CppFile) },
    HighlightingTypes = new[] { typeof(IwyuHighlighting) })]
public class IwyuAnalyzer : IElementProblemAnalyzer
{
    public void Run(ITreeNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        var file = element as CppFile ?? throw new InvalidOperationException();
        var usedIncludes = UsedIncludesProcessor.CollectUsedIncludes(file).ToArray();
        return;
    }
}
