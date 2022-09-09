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
        public const string GeneratedInterfacesNamespace = "AutoDependencies.Interfaces.Generated";
    }

    public const string GeneratedDocumentExtension = ".g.cs";
}
