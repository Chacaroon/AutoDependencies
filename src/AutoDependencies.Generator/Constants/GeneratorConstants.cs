namespace AutoDependencies.Generator.Constants;
public static class GeneratorConstants
{
    public static class AttributeNames
    {
        public const string GeneratedAttribute = "GeneratedAttribute";
        public const string ServiceAttribute = "ServiceAttribute";
        public const string InjectAttribute = "InjectAttribute";
        public const string ServiceConstructorAttribute = "ServiceConstructorAttribute";
    }

    public static class PredefinedNamespaces
    {
        public const string AttributesNamespace = "AutoDependencies.Attributes";
        public const string DependencyInjectionNamespace = "Microsoft.Extensions.DependencyInjection";
        public const string GeneratedExtensionsNamespacePart = "Extensions.Generated";
        public const string GeneratedInterfacesNamespacePart = "Interfaces.Generated";
    }

    public static class PredefinedClassNames
    {
        public const string ServiceCollectionExtensions = "ServiceCollectionExtensions";
    }

    public const string GeneratedDocumentExtension = ".g.cs";
}
