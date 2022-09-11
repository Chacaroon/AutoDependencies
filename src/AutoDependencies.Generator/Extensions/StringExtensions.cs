namespace AutoDependencies.Generator.Extensions;
public static class StringExtensions
{
    public static string ToGeneratedFileName(this string name)
    {
        if (name.EndsWith(GeneratedDocumentExtension))
        {
            return name;
        }

        if (name.EndsWith(".cs"))
        {
            return name.Replace(".cs", GeneratedDocumentExtension);
        }

        return $"{name}{GeneratedDocumentExtension}";
    }
}
