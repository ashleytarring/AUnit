using System.ComponentModel;
using System.Reflection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using DUnit.TestAdapter.Navigation;

namespace DUnit.TestAdapter;
[FileExtension(".exe")]
[FileExtension(".dll")]
[DefaultExecutorUri(TestExecutor.ExecutorUri)]
[Category("managed")]
public class TestDiscoverer : ITestDiscoverer
{
  public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
  {
    foreach (var source in sources)
    {
      var assembly = Assembly.LoadFrom(source);
      foreach (var type in assembly.GetTypes())
      {
        if (type.GetCustomAttributes(typeof(TestFixtureAttribute), true).Any())
        {
          foreach (var method in type.GetMethods())
          {
            if (method.GetCustomAttributes(typeof(TestAttribute), true).Any())
            {
              var testCase = CreateTestCase(source, type, method);
              discoverySink.SendTestCase(testCase);
            }
          }
        }
      }
    }
  }

  private TestCase CreateTestCase(string source, Type type, MethodInfo method)
  {
    using var navigationDataProvider = new NavigationDataProvider(method.DeclaringType!.Assembly.Location);
    var navData = navigationDataProvider.GetNavigationData(method.DeclaringType.FullName!, method.Name);

    if (navData is null || !navData.IsValid){
      navData = new NavigationData("unknown.cs", 0);
    }

    var fullyQualifiedName = $"{type.FullName}.{method.Name}";

    return new TestCase(fullyQualifiedName, new Uri(TestExecutor.ExecutorUri), source)
    {
      DisplayName = method.Name,
      CodeFilePath = navData.FilePath,
      LineNumber = navData.LineNumber
    };
  }
}
