using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Cpp.CodeStyle.IncludesOrder;
using JetBrains.ReSharper.Feature.Services.Cpp.QuickFixes;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.Cpp.Caches;
using JetBrains.ReSharper.Psi.Cpp.Tree;
using JetBrains.ReSharper.Psi.Cpp.Tree.Util;
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
public class RecomputeIncludesHighlighting : IHighlighting
{
    private const string Message = "IWYU: Wrong inclusions";

    public readonly ImportDirective ImportDirective;
    public readonly CppFileLocation[] UsedIncludes;

    public RecomputeIncludesHighlighting(ImportDirective importDirective, CppFileLocation[] usedIncludes)
    {
        ImportDirective = importDirective;
        UsedIncludes = usedIncludes;
    }

    public string ToolTip => "Wrong inclusions";
    public string ErrorStripeToolTip => ToolTip;
    public bool IsValid() => ImportDirective.IsValid();
    public DocumentRange CalculateRange() => ImportDirective.GetDocumentRange();
}

public class RecomputeIncludesHighlightingFix : CppQuickFixBase
{
    private readonly RecomputeIncludesHighlighting _highlighting;

    public override string Text => "Recompute IWYU includes";

    public RecomputeIncludesHighlightingFix(RecomputeIncludesHighlighting highlighting)
    {
        _highlighting = highlighting;
    }

    public override bool IsAvailable(IUserDataHolder cache)
    {
        return _highlighting.IsValid();
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        var containingFile = _highlighting.ImportDirective.GetContainingFile() ?? throw new InvalidOperationException();

        ISet<string> desiredIncludes = new HashSet<string>();
        var currentImports = containingFile.Children().OfType<ImportDirective>().ToList();

        foreach (var usedInclude in _highlighting.UsedIncludes)
        {
            var includeDirective = CppQuickFixUtil.GetIncludeDirectivePath(containingFile, usedInclude);
            if (containingFile.InsertImportDirectiveIfMissing(includeDirective) == null)
                throw new FailPsiTransactionException("Failed to insert ImportDirective");
            desiredIncludes.Add(includeDirective);
        }

        // remove unused includes
        foreach (var directive in currentImports)
        {
            var quotedImportPath = directive.QuotedImportPath;
            if (!quotedImportPath.IsEmpty() && !desiredIncludes.Contains(quotedImportPath.GetNodeText()))
            {
                if (CppModificationUtil.CanBeDeleted(directive))
                {
                    CppModificationUtil.DeleteChild(directive);
                }
            }
        }

        return null;
    }
}
