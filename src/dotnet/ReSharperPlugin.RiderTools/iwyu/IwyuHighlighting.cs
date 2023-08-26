using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.Cpp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharperPlugin.RiderTools.iwyu;

[StaticSeverityHighlighting(Severity.INFO,
    typeof(HighlightingGroupIds.IdentifierHighlightings),
    Languages = "CPP",
    AttributeId = AnalysisHighlightingAttributeIds.WARNING,
    OverlapResolve = OverlapResolveKind.WARNING,
    ToolTipFormatString = Message)]
public class IwyuHighlighting : IHighlighting
{
    private const string Message = "Wrong inclusions";

    private readonly ImportDirective _importDirective;

    public IwyuHighlighting(ImportDirective importDirective)
    {
        _importDirective = importDirective;
    }

    public string ToolTip => "Wrong inclusions";
    public string ErrorStripeToolTip => ToolTip;
    public bool IsValid() => _importDirective.IsValid();
    public DocumentRange CalculateRange() => _importDirective.GetDocumentRange();
}
