using DUnit.Core;

namespace UnitTests.Tests;

[TestFixture]
public class SampleTests
{
  [Test]
  public void TestMethod1()
  {
    // Simple assertion to check the test framework
    if (1 != 1)
    {
      throw new Exception("TestMethod1 failed!");
    }
  }

  [Test]
  public void TestMethod2()
  {
    // Another simple test case
    if (2 + 2 != 4)
    {
      throw new Exception("TestMethod2 failed!");
    }
  }
}