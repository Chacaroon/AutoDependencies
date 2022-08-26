using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

namespace AutoDependencies.ConsoleApp;
internal static class WorkspaceFactory
{
    private static MSBuildWorkspace? _workspace;

    static WorkspaceFactory()
    {
        MSBuildLocator.RegisterDefaults();
    }

    public static MSBuildWorkspace GetOrCreateWorkspace()
    {
        if (_workspace != null) 
            return _workspace;

        _workspace = MSBuildWorkspace.Create();

        return _workspace;
    }
}
