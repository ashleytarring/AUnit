namespace DUnit.TestAdapter.Navigation;

public class NavigationData
{
    public NavigationData(string filePath, int lineNumber)
    {
        FilePath = filePath;
        LineNumber = lineNumber;
    }

    public string FilePath { get; }

    public int LineNumber { get; }

    public bool IsValid => !string.IsNullOrEmpty(FilePath);
}