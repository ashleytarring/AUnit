using System.Reflection;

namespace DUnit.TestAdapter.Navigation;

public readonly struct TypeInfo(string assemblyPath, string fullName)
{
  public TypeInfo(Type type) : this(type.GetTypeInfo().Assembly.Location, type.FullName!)
  {
  }

  public string AssemblyPath { get; } = assemblyPath;
  public string FullName { get; } = fullName;
}