using DUnit;

namespace UnitTests.Tests;

[TestFixture]
public class SampleTests
{
  [Test]
  public void TestMethod1()
  {
    Assert.Equal(expected: 1, actual: 1);
  }

  [Test]
  public void TestMethod2()
  {
    Assert.Pass();
  }

  [Test]
  public void FailingTestMethod1()
  {
    Assert.Equal(expected: 1, actual: 2);
  }
}