namespace DUnit.TestAdapter.Navigation;

public interface IMetadataProvider : IDisposable
{
  TypeInfo? GetDeclaringType(string assemblyPath, string reflectedTypeName, string methodName);
  TypeInfo? GetStateMachineType(string assemblyPath, string reflectedTypeName, string methodName);
}