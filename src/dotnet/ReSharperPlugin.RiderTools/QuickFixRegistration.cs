using System;
using System.Collections.Generic;
using JetBrains.Application;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using ReSharperPlugin.RiderTools.iwyu;

namespace ReSharperPlugin.RiderTools;

[ShellComponent]
public class QuickFixRegistration : IQuickFixesProvider
{
    public IEnumerable<Type> Dependencies => Array.Empty<Type>();

    public void Register(IQuickFixesRegistrar table)
    {
        table.RegisterQuickFix<RecomputeIncludesHighlighting>(Lifetime.Eternal,
            highlighting => new RecomputeIncludesHighlightingFix(highlighting),
            typeof(RecomputeIncludesHighlightingFix));

        table.RegisterQuickFix<IwyuSymbolNotImported>(Lifetime.Eternal,
            highlighting => new IwyuSymbolNotImportedFix(highlighting),
            typeof(IwyuSymbolNotImportedFix));
    }
}
