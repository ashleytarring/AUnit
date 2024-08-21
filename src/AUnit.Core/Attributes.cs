namespace AUnit.Core;

[AttributeUsage(AttributeTargets.Class)]
public class TestFixtureAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
public class TestAttribute : Attribute
{
}