using System.Reflection;

namespace DUnit.TestAdapter.Navigation;

internal sealed class DirectReflectionMetadataProvider : IMetadataProvider
{
  public TypeInfo? GetDeclaringType(string assemblyPath, string reflectedTypeName, string methodName)
  {
    var type = TryGetSingleMethod(assemblyPath, reflectedTypeName, methodName)?.DeclaringType;
    if (type == null) return null;

    if (type.IsConstructedGenericType)
    {
      type = type.GetGenericTypeDefinition();
    }

    return new TypeInfo(type);
  }

  public TypeInfo? GetStateMachineType(string assemblyPath, string reflectedTypeName, string methodName)
  {
    var method = TryGetSingleMethod(assemblyPath, reflectedTypeName, methodName);
    if (method == null) return null;

    var candidate = (Type?)null;

    foreach (var attributeData in CustomAttributeData.GetCustomAttributes(method))
    {
      for (var current = attributeData.Constructor.DeclaringType; current != null; current = current.GetTypeInfo().BaseType)
      {
        if (current.FullName != "System.Runtime.CompilerServices.StateMachineAttribute") continue;

        var parameters = attributeData.Constructor.GetParameters();
        for (var i = 0; i < parameters.Length; i++)
        {
          if (parameters[i].Name != "stateMachineType") continue;
          if (attributeData.ConstructorArguments[i].Value is Type argument)
          {
            if (candidate != null)
              return null;
            candidate = argument;
          }
        }
      }
    }

    if (candidate == null)
      return null;
    return new TypeInfo(candidate);
  }

  private static MethodInfo? TryGetSingleMethod(string assemblyPath, string reflectedTypeName, string methodName)
  {
    try
    {
      var assembly = Assembly.LoadFrom(assemblyPath);

      var type = assembly.GetType(reflectedTypeName, throwOnError: false);

      var methods = type?.GetMethods().Where(m => m.Name == methodName).Take(2).ToList();
      return methods?.Count == 1 ? methods[0] : null;
    }
    catch (FileNotFoundException)
    {
      return null;
    }
  }

  void IDisposable.Dispose()
  {
  }
}