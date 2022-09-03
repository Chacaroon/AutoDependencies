using AutoDependencies.Generator.Constants;

namespace AutoDependencies.Generator.Extensions;
public static class StringExtensions
{
    public static string ToGeneratedFileName(this string name)
    {
        if (name.EndsWith(Constants.GeneratorConstants.GeneratedDocumentExtension))
        {
            return name;
        }

        if (name.EndsWith(".cs"))
        {
            name = name.Replace(".cs", string.Empty);
        }

        return $"{name}{Constants.GeneratorConstants.GeneratedDocumentExtension}";
    }
}
