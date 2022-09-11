namespace AutoDependencies.Generator.Constants;
public class GeneratorConstants
{
    public class AttributeNames
    {
        public const string GeneratedAttribute = "GeneratedAttribute";
        public const string ServiceAttribute = "ServiceAttribute";
        public const string InjectAttribute = "InjectAttribute";
    }

    public class PredefinedNamespaces
    {
        public const string AttributesNamespace = "AutoDependencies.Attributes";
        public const string DependencyInjectionNamespace = "Microsoft.Extensions.DependencyInjection";
        public const string GeneratedExtensionsNamespacePart = "Extensions.Generated";
    }

    public class PredefinedClassNames
    {
        public const string ServiceCollectionExtensions = "ServiceCollectionExtensions";
    }

    public const string GeneratedDocumentExtension = ".g.cs";
}
