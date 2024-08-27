using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace DUnit.TestAdapter.Navigation;

public sealed class NavigationDataProvider : IDisposable
{
  private readonly string _assemblyPath;
  private readonly IMetadataProvider _metadataProvider;
  private readonly Dictionary<string, DiaSession> _sessionsByAssemblyPath = new(StringComparer.OrdinalIgnoreCase);
  private bool _disableMetadataLookup;

  public NavigationDataProvider(string assemblyPath) : this(assemblyPath, new DirectReflectionMetadataProvider()) { }

  internal NavigationDataProvider(string assemblyPath, IMetadataProvider metadataProvider)
  {
    if (string.IsNullOrEmpty(assemblyPath))
      throw new ArgumentException("Assembly path must be specified.", nameof(assemblyPath));

    _assemblyPath = assemblyPath;
    _metadataProvider = metadataProvider ?? throw new ArgumentNullException(nameof(metadataProvider));
  }

  public void Dispose()
  {
    _metadataProvider.Dispose();

    foreach (var session in _sessionsByAssemblyPath.Values)
      session?.Dispose();
  }

  public NavigationData? GetNavigationData(string className, string methodName)
  {
    return TryGetSessionData(_assemblyPath, className, methodName)
      ?? TryGetSessionData(DoWithBreaker(_metadataProvider.GetStateMachineType, className, methodName), "MoveNext")
      ?? TryGetSessionData(DoWithBreaker(_metadataProvider.GetDeclaringType, className, methodName), methodName);
  }

  private TypeInfo? DoWithBreaker(Func<string, string, string, TypeInfo?> method, string declaringTypeName, string methodName)
  {
    if (_disableMetadataLookup)
      return null;
    try
    {
      return method.Invoke(_assemblyPath, declaringTypeName, methodName);
    }
    catch
    {
      _disableMetadataLookup = true;
    }
    return null;
  }

  private NavigationData? TryGetSessionData(string assemblyPath, string declaringTypeName, string methodName)
  {
    if (!_sessionsByAssemblyPath.TryGetValue(assemblyPath, out var session))
    {
      try
      {
        session = new DiaSession(assemblyPath);
      }
      catch
      {
        session = null;
      }
      _sessionsByAssemblyPath.Add(assemblyPath, session!);
    }

    var data = session?.GetNavigationData(declaringTypeName, methodName);

    return string.IsNullOrEmpty(data?.FileName) ? null :
        new NavigationData(data.FileName, data.MinLineNumber);
  }

  private NavigationData? TryGetSessionData(TypeInfo? declaringType, string methodName)
  {
    return declaringType == null ? null :
        TryGetSessionData(declaringType.Value.AssemblyPath, declaringType.Value.FullName, methodName);
  }
}