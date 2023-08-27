using System;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.DocumentModel;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Cpp.Language;
using JetBrains.ReSharper.Psi.Cpp.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReSharperPlugin.RiderToys.iwyu;

namespace ReSharperPlugin.RiderToys.Tests.iwyu;

[TestFileExtension(".cpp")]
public class UsedIncludesProcessorTests : BaseTestWithSingleProject
{
    [TestCase("MacroCall1")]
    [TestCase("MacroCall2")]
    [TestCase("MethodDefinition1")]
    [TestCase("MethodDefinition2")]
    [TestCase("MethodDefinition3")]
    public void InvokeTest(string name) => DoOneTest(name);

    protected override void DoTest(Lifetime lifetime, IProject testProject)
    {
        foreach (var projectFile in testProject.GetAllProjectFiles())
        {
            CheckFile(projectFile.ToSourceFile().NotNull("sourceFile != null"));
        }
    }

    private void CheckFile([NotNull] IPsiSourceFile sourceFile)
    {
        ExecuteWithGold(sourceFile, textWriter =>
        {
            var file = sourceFile.GetPsiFile<CppLanguage>(new DocumentOffset(sourceFile.Document, 0)) as CppFile;
            var includes = UsedIncludesProcessor.CollectUsedIncludes(file.NotNull())
                .Select(include => include.Location.MakeRelativeTo(sourceFile.GetLocation().Parent))
                .Sort((x, y) => String.Compare(x.FullPath, y.FullPath, StringComparison.Ordinal))
                .ToList();

            foreach (var include in includes)
            {
                textWriter.WriteLine(include.FullPath);
            }
        });
    }
}
