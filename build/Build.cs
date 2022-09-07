using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[ShutdownDotNetAfterServerBuild]
[GitHubActions("BuildAndPublish",
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = true,
    FetchDepth = 0,
    OnPushTags = new[] { "*" },
    OnPushBranches = new[] { "master", "main" },
    OnPullRequestBranches = new[] { "*" },
    CacheExcludePatterns = new[] { "~/.nuget/packages/autodependencies.generator" },
    ImportSecrets = new[] { nameof(NuGetToken) },
    InvokedTargets = new[] { nameof(Clean), nameof(Test), nameof(TestPackage), nameof(PushToNuGet) })]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Compile, x => x.Pack, x => x.TestPackage);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution(GenerateProjects = true)] readonly Solution Solution;
    [GitVersion(Framework = "net6.0")] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    [Parameter, Secret] readonly string NuGetToken;
    [Parameter] readonly AbsolutePath PackagesDirectory = RootDirectory / "packages";

    const string NugetOrgUrl = "https://api.nuget.org/v3/index.json";
    bool IsTag => GitHubActions.Instance?.Ref?.StartsWith("refs/tags/") ?? false;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            Logger.LogLevel = LogLevel.Trace;

            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            if (!string.IsNullOrEmpty(PackagesDirectory))
            {
                EnsureCleanDirectory(PackagesDirectory);
            }
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .When(!string.IsNullOrEmpty(PackagesDirectory), x => x.SetPackageDirectory(PackagesDirectory))
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .When(IsServerBuild, x => x.SetProperty("ContinuousIntegrationBuild", "true"))
                .SetVersion(IsTag ? GitVersion.MajorMinorPatch : "1.0.0-build")
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore());
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .After(Test)
        .Produces(ArtifactsDirectory)
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetConfiguration(Configuration)
                .SetOutputDirectory(ArtifactsDirectory)
                .EnableNoBuild()
                .EnableNoRestore()
                .When(IsServerBuild, x => x.SetProperty("ContinuousIntegrationBuild", "true"))
                .SetProject(Solution));
        });

    Target TestPackage => _ => _
        .DependsOn(Pack)
        .After(Test)
        .Produces(ArtifactsDirectory)
        .Executes(() =>
        {
            var projectFiles = new[]
            {
                Solution.tests.AutoDepencencies_NugetIntegrationTests,
            };

            if (!string.IsNullOrEmpty(PackagesDirectory))
            {
                DeleteDirectory(PackagesDirectory / "autodependencies.generator");
            }

            DotNetRestore(s => s
                .When(!string.IsNullOrEmpty(PackagesDirectory), x => x.SetPackageDirectory(PackagesDirectory))
                .SetConfigFile(RootDirectory / "nuget.integration-tests.config")
                .CombineWith(projectFiles, (s, p) => s.SetProjectFile(p)));

            DotNetBuild(s => s
                .When(!string.IsNullOrEmpty(PackagesDirectory), x => x.SetPackageDirectory(PackagesDirectory))
                .EnableNoRestore()
                .SetConfiguration(Configuration)
                .CombineWith(projectFiles, (s, p) => s.SetProjectFile(p)));

            DotNetTest(s => s
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore()
                .CombineWith(projectFiles, (s, p) => s.SetProjectFile(p)));
        });

    Target PushToNuGet => _ => _
        .DependsOn(Pack)
        .OnlyWhenStatic(() => IsTag && IsServerBuild)
        .Requires(() => NuGetToken)
        .After(TestPackage)
        .Executes(() =>
        {
            Log.Information("Push to NuGet! Version = {Version}", GitVersion.MajorMinorPatch);

            //var packages = ArtifactsDirectory.GlobFiles("*.nupkg");
            //DotNetNuGetPush(s => s
            //    .SetApiKey(NuGetToken)
            //    .SetSource(NugetOrgUrl)
            //    .EnableSkipDuplicate()
            //    .CombineWith(packages, (x, package) => x
            //        .SetTargetPath(package)));
        });
}
