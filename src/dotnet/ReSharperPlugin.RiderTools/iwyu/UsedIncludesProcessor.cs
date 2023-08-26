using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Cpp.Caches;
using JetBrains.ReSharper.Psi.Cpp.Symbols;
using JetBrains.ReSharper.Psi.Cpp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharperPlugin.RiderTools.iwyu;

/**
 * Collects the full paths of all symbols used in a file.
 *
 * Es:
 * <code>
 * #include "foo.h" // exposes foo()
 * #include "bar.h" // exposes bar()
 *
 * int main() {
 *    foo();
 *    baz();  // defined in baz.h which is included in bar.h
 *    return 0;
 * }
 * </code>
 *
 * will return ["foo.h", "baz.h"]
 * - "baz.h" is returned also if it is indirectly included
 * - "bar.h" is not returned as it doesn't expose any symbol used in the file
 */
public class UsedIncludesProcessor : IRecursiveElementProcessor
{
    private readonly string _fileFullPath;
    private readonly ISet<CppFileLocation> _result;

    /**
     * Collects all includes used in a file.
     * @param file the file to analyze
     */
    public static List<CppFileLocation> CollectUsedIncludes(CppFile file)
    {
        ISet<CppFileLocation> result = new HashSet<CppFileLocation>();
        file.ProcessDescendants(new UsedIncludesProcessor(file.File.FullPath, result));
        return result.ToList();
    }

    private UsedIncludesProcessor(string fileFullPath, ISet<CppFileLocation> result)
    {
        _fileFullPath = fileFullPath;
        _result = result;
    }

    public bool ProcessingIsFinished => false;

    public bool InteriorShouldBeProcessed(ITreeNode element)
    {
        switch (element)
        {
            case ImportDirective:
            case MacroDefinition:
            case MacroReference:
                return false;
            default:
                return true;
        }
    }

    public void ProcessBeforeInterior(ITreeNode element)
    {
        if (element is BaseQualifiedReference reference)
        {
            // In case of "a::b::c", ignore "a::b::" part as it is implicitly included with full name
            if (reference is NameQualifier)
            {
                return;
            }

            var entity = reference.GetCppResolveResult().GetPrimaryEntity();
            if (entity != null)
            {
                foreach (var part in entity.SymbolParts)
                {
                    ProcessLocation(part.Location);
                }
            }
        }

        if (element is MacroCall macroCall && macroCall.IsTopLevel())
        {
            var macro = macroCall.MacroReferenceNode?.GetReferencedSymbol();;
            if (macro != null)
            {
                ProcessLocation(macro.Location);
            }
        }
    }

    private void ProcessLocation(CppSymbolLocation partLocation)
    {
        if (_fileFullPath != partLocation.ContainingFile.FullPath)
        {
            _result.Add(partLocation.ContainingFile);
        }
    }

    public void ProcessAfterInterior(ITreeNode element)
    {
    }
}
