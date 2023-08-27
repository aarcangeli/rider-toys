using System;
using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Cpp.CodeStyle.IncludesOrder;
using JetBrains.ReSharper.Feature.Services.Cpp.QuickFixes;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.Cpp.Caches;
using JetBrains.ReSharper.Psi.Cpp.Tree;
using JetBrains.ReSharper.Psi.Transactions;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReSharperPlugin.RiderToys.iwyu;

[StaticSeverityHighlighting(Severity.INFO,
    typeof(HighlightingGroupIds.IdentifierHighlightings),
    Languages = "CPP",
    AttributeId = AnalysisHighlightingAttributeIds.WARNING,
    OverlapResolve = OverlapResolveKind.WARNING,
    ToolTipFormatString = Message)]
public class IwyuSymbolNotImported : IHighlighting
{
    private const string Message = "IWYU: Symbol not imported";

    public readonly CppCompositeNode Element;
    public readonly CppFileLocation Location;

    public IwyuSymbolNotImported(CppCompositeNode element, CppFileLocation location)
    {
        Element = element;
        Location = location;
    }

    public string ToolTip => Message;
    public string ErrorStripeToolTip => ToolTip;
    public bool IsValid() => Element.IsValid();
    public DocumentRange CalculateRange() => Element.GetDocumentRange();
}

public class IwyuSymbolNotImportedFix : CppQuickFixBase
{
    private readonly IwyuSymbolNotImported _highlighting;

    public IwyuSymbolNotImportedFix(IwyuSymbolNotImported highlighting)
    {
        _highlighting = highlighting;
    }

    public override string Text => GetText();
    public override bool IsAvailable(IUserDataHolder cache) => _highlighting.IsValid();

    private string GetText()
    {
        var containingFile = _highlighting.Element.GetContainingFile() ?? throw new InvalidOperationException();

        var includeDirective = CppQuickFixUtil.GetIncludeDirectivePath(containingFile, _highlighting.Location);
        return !includeDirective.IsEmpty() ? $"Add #include {includeDirective}" : "Add missing #include";
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        var containingFile = _highlighting.Element.GetContainingFile() ?? throw new InvalidOperationException();

        var includeDirective = CppQuickFixUtil.GetIncludeDirectivePath(containingFile, _highlighting.Location);
        if (containingFile.InsertImportDirectiveIfMissing(includeDirective) == null)
            throw new FailPsiTransactionException("Failed to insert ImportDirective");

        return null;
    }
}
