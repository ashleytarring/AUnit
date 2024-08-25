﻿using System.Reflection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using DUnit.Core;

namespace DUnit.TestAdapter;

[ExtensionUri(ExecutorUri)]
public class TestExecutor : ITestExecutor
{
  public const string ExecutorUri = "executor://TestExecutor";
  private bool _isCanceled;

  public void Cancel()
  {
    _isCanceled = true;
  }

  public void RunTests(IEnumerable<TestCase>? tests, IRunContext? runContext, IFrameworkHandle? frameworkHandle)
  {
    if (tests == null || frameworkHandle == null)
    {
      throw new ArgumentNullException(nameof(tests), "Test cases and framework handle cannot be null.");
    }

    // Iterate through the discovered test cases and execute them.
    foreach (var test in tests)
    {
      if (_isCanceled)
      {
        break;
      }

      ExecuteTest(test, frameworkHandle);
    }
  }

  public void RunTests(IEnumerable<string>? sources, IRunContext? runContext, IFrameworkHandle? frameworkHandle)
  {
    if (sources == null || frameworkHandle == null)
    {
      throw new ArgumentNullException(nameof(sources), "Sources and framework handle cannot be null.");
    }

    var testCases = new List<TestCase>();

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
              var fullyQualifiedName = $"{type.FullName}.{method.Name}";
              var testCase = new TestCase(fullyQualifiedName, new Uri(ExecutorUri), source)
              {
                DisplayName = $"{type.FullName}.{method.Name}",
                CodeFilePath = method.DeclaringType?.Assembly.Location,
                LineNumber = 0,
                FullyQualifiedName = fullyQualifiedName // Ensure this is set correctly
              };

              testCases.Add(testCase);
            }
          }
        }
      }
    }

    RunTests(testCases, runContext, frameworkHandle);
  }

  private void ExecuteTest(TestCase test, IFrameworkHandle frameworkHandle)
  {
    frameworkHandle.RecordStart(test);

    try
    {
      var assembly = Assembly.LoadFrom(test.Source);
      var type = assembly.GetType(test.FullyQualifiedName.Substring(0, test.FullyQualifiedName.LastIndexOf('.')));
      var method = type?.GetMethod(test.FullyQualifiedName.Substring(test.FullyQualifiedName.LastIndexOf('.') + 1));

      if (method != null)
      {
        var instance = Activator.CreateInstance(type!);
        method.Invoke(instance, null);

        frameworkHandle.RecordResult(new TestResult(test)
        {
          Outcome = TestOutcome.Passed
        });
      }
      else
      {
        frameworkHandle.RecordResult(new TestResult(test)
        {
          Outcome = TestOutcome.Failed,
          ErrorMessage = "Test method not found."
        });
      }
    }
    catch (Exception ex)
    {
      frameworkHandle.RecordResult(new TestResult(test)
      {
        Outcome = TestOutcome.Failed,
        ErrorMessage = ex.Message,
        ErrorStackTrace = ex.StackTrace
      });
    }

    frameworkHandle.RecordEnd(test, TestOutcome.Passed);
  }
}
