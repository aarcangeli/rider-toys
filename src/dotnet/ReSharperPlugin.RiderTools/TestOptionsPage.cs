using JetBrains.Annotations;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.IDE.UI.Options;
using JetBrains.Lifetimes;

namespace ReSharperPlugin.RiderTools;

[OptionsPage("RiderTools.TestOptionsPage", "My Option Name", null)]
public class TestOptionsPage : BeSimpleOptionsPage
{
    public TestOptionsPage(Lifetime lifetime, OptionsPageContext optionsPageContext,
        OptionsSettingsSmartContext optionsSettingsSmartContext,
        TestComponent testComponent,
        TestSolutionComponent solutionComponent = null) : base(
        lifetime, optionsPageContext,
        optionsSettingsSmartContext)
    {
        AddCommentText("Example comment");
        AddText(solutionComponent != null ? "Solution component is not null" : "Solution component is null");
    }
}
