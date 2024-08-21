using System.Reflection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using AUnit.Core;

namespace AUnit.Adapter;
public class TestDiscovery : ITestDiscoverer
{
  public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
  {
    foreach (var source in sources)
    {
      Assembly assembly = Assembly.LoadFrom(source);

      foreach (var type in assembly.GetTypes())
      {
        if (type.GetCustomAttributes(typeof(TestFixtureAttribute), true).Any())
        {
          foreach (var method in type.GetMethods())
          {
            if (method.GetCustomAttributes(typeof(TestAttribute), true).Any())
            {
              var testCase = new TestCase(
                  method.Name,
                  new Uri(TestExecution.ExecutorUriString),
                  source)
              {
                DisplayName = $"{type.Name}.{method.Name}",
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
