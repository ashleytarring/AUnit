using System;

namespace DUnit;
public class Assert
{
  /// <summary>
  /// Marks the test as passed without performing any checks.
  /// </summary>
  public static void Pass()
  {
    // This would ideally hook into the test framework's infrastructure to mark the test as passed.
    // Here we are simply throwing an exception to simulate the behavior.
    throw new TestPassedException();
  }

  /// <summary>
  /// Asserts that two values are equal.
  /// </summary>
  /// <typeparam name="T">The type of the objects being compared.</typeparam>
  /// <param name="expected">The expected value.</param>
  /// <param name="actual">The actual value.</param>
  public static void Equal<T>(T expected, T actual)
  {
    if (Equals(expected, actual))
    {
      throw new TestPassedException();
    }
    throw new TestFailedException($"Assert.Equal failed: Expected:<{expected}> Actual:<{actual}>");
  }
}

/// <summary>
/// Exception to signal that a test has passed.
/// </summary>
public class TestPassedException : Exception
{
  public TestPassedException() : base()
  {
  }
}

/// <summary>
/// Exception to signal that a test has failed.
/// </summary>
public class TestFailedException : Exception
{
  public TestFailedException(string message) : base(message)
  {
  }
}