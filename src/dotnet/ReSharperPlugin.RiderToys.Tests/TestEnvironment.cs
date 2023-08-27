using System.Threading;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;

[assembly: Apartment(ApartmentState.STA)]

namespace ReSharperPlugin.RiderToys.Tests
{
    [ZoneDefinition]
    public class RiderToysTestEnvironmentZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>, IRequire<IRiderToysZone> { }

    [ZoneMarker]
    public class ZoneMarker : IRequire<ICodeEditingZone>, IRequire<ILanguageCSharpZone>, IRequire<RiderToysTestEnvironmentZone> { }

    [SetUpFixture]
    public class RiderToysTestsAssembly : ExtensionTestEnvironmentAssembly<RiderToysTestEnvironmentZone> { }
}
