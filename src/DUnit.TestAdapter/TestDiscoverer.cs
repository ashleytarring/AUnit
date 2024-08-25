using System.Reflection;
using System.ComponentModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using DUnit.Core;

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
              var testCase = new TestCase(
                $"{type.FullName}.{method.Name}",
                new Uri(TestExecutor.ExecutorUri),
                source)
              {
                DisplayName = method.Name,
                CodeFilePath = method.DeclaringType?.Assembly.Location,
                LineNumber = 0
              };
              discoverySink.SendTestCase(testCase);
            }
          }
        }
      }
    }
  }
}
