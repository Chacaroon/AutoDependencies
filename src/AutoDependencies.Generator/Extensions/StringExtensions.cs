using AutoDependencies.Generator.Constants;

namespace AutoDependencies.Generator.Extensions;
public static class StringExtensions
{
    public static string ToGeneratedFileName(this string name)
    {
        if (name.EndsWith(GeneratorConstants.GeneratedDocumentExtension))
        {
            return name;
        }

        if (name.EndsWith(".cs"))
        {
            return name.Replace(".cs", GeneratorConstants.GeneratedDocumentExtension);
        }

        return $"{name}{GeneratorConstants.GeneratedDocumentExtension}";
    }
}
