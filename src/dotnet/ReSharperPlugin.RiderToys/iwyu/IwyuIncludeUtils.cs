using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.Cpp.CodeStyle.IncludesOrder;
using JetBrains.ReSharper.Psi.Cpp.Caches;
using JetBrains.ReSharper.Psi.Cpp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace ReSharperPlugin.RiderToys.iwyu;

public class IwyuIncludeUtils
{
    public static CppFileLocation[] FindAllIncludesPaths(CppFile file)
    {
        ISet<CppFileLocation> result = new HashSet<CppFileLocation>();

        foreach (var directive in file.Children().OfType<ImportDirective>())
        {
            var target = directive.GetSymbol().Target;
            if (target.IsValid())
            {
                result.Add(target);
            }
        }

        return result.ToArray();
    }
}
