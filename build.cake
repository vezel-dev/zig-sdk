#addin nuget:?package=Cake.DoInDirectory&version=6.0.0
#addin nuget:?package=Cake.Npm&version=2.0.0

#nullable enable

// Arguments

var target = Argument("t", "default");

// Environment

var githubToken = EnvironmentVariable("GITHUB_TOKEN");
var nugetToken = EnvironmentVariable("NUGET_TOKEN");

// Paths

var root = Context.Environment.WorkingDirectory;
var zigSdkProj = root.CombineWithFilePath("zig-sdk.proj");
var samplesProj = root.Combine("src").Combine("samples").CombineWithFilePath("samples.proj");
var doc = root.Combine("doc");
var @out = root.Combine("out");
var outLogDotnet = @out.Combine("log").Combine("dotnet");
var outPkg = @out.Combine("pkg");

// Globs

var githubGlob = new GlobPattern(outPkg.Combine("release").CombineWithFilePath("*.nupkg").FullPath);
var nugetGlob = new GlobPattern(outPkg.Combine("release").CombineWithFilePath("*.nupkg").FullPath);

// Utilities

DotNetMSBuildSettings ConfigureMSBuild(string category, string target)
{
    var prefix = $"{category}_{target}_{Environment.UserName}_{Environment.MachineName}_";
    var time = DateTime.Now;

    string name;

    do
    {
        name = $"{prefix}{time:yyyy-MM-dd_HH_mm_ss}.binlog";
        time = time.AddSeconds(1);
    }
    while (System.IO.File.Exists(name));

    return new()
    {
        // TODO: https://github.com/dotnet/msbuild/issues/6756
        NoLogo = true,
        BinaryLogger = new()
        {
            Enabled = true,
            FileName = outLogDotnet.CombineWithFilePath(name).FullPath,
        },
        ConsoleLoggerSettings = new()
        {
            NoSummary = true,
        },
        ArgumentCustomization = args => args.Append("-ds:false"),
    };
}

// Tasks

Task("default")
    .IsDependentOn("test")
    .IsDependentOn("build");

Task("default-editor")
    .IsDependentOn("build")
    .IsDependentOn("pack");

Task("restore-core")
    .Does(() =>
        DotNetRestore(
            zigSdkProj.FullPath,
            new()
            {
                MSBuildSettings = ConfigureMSBuild("core", "restore"),
            }));

Task("restore-doc")
    .Does(() => DoInDirectory(doc, () => NpmInstall()));

Task("restore")
    .IsDependentOn("restore-core")
    .IsDependentOn("restore-doc");

Task("build-core")
    .IsDependentOn("restore-core")
    .Does(() =>
        DotNetBuild(
            zigSdkProj.FullPath,
            new()
            {
                MSBuildSettings = ConfigureMSBuild("core", "build"),
                NoRestore = true,
            }));

Task("build-doc")
    .IsDependentOn("restore-doc")
    .Does(() => DoInDirectory(doc, () => NpmRunScript("build")));

Task("build")
    .IsDependentOn("build-core")
    .IsDependentOn("build-doc");

Task("pack-core")
    .IsDependentOn("build-core")
    .Does(() =>
        DotNetPack(
            zigSdkProj.FullPath,
            new()
            {
                MSBuildSettings = ConfigureMSBuild("core", "pack"),
                NoBuild = true,
            }));

Task("pack")
    .IsDependentOn("pack-core");

Task("test-samples-restore")
    .IsDependentOn("pack-core")
    .Does(() =>
        DotNetRestore(
            samplesProj.FullPath,
            new()
            {
                MSBuildSettings = ConfigureMSBuild("samples", "restore"),
            }));

Task("test-samples-build")
    .IsDependentOn("test-samples-restore")
    .Does(() =>
    {
        foreach (var cfg in new[] { "Debug", "Release" })
            DotNetBuild(
                samplesProj.FullPath,
                new()
                {
                    MSBuildSettings = ConfigureMSBuild("samples", "build"),
                    Configuration = cfg,
                    NoRestore = true,
                });
    });

Task("test-samples-pack")
    .IsDependentOn("test-samples-build")
    .Does(() =>
    {
        foreach (var cfg in new[] { "Debug", "Release" })
            DotNetPack(
                samplesProj.FullPath,
                new()
                {
                    MSBuildSettings = ConfigureMSBuild("samples", "pack"),
                    Configuration = cfg,
                    NoBuild = true,
                });
    });

Task("test-samples-publish")
    .IsDependentOn("test-samples-build")
    .Does(() =>
    {
        foreach (var cfg in new[] { "Debug", "Release" })
            DotNetPublish(
                samplesProj.FullPath,
                new()
                {
                    MSBuildSettings = ConfigureMSBuild("samples", "publish"),
                    Configuration = cfg,
                    NoBuild = true,
                });
    });

Task("test-samples-test")
    .IsDependentOn("test-samples-restore")
    .Does(() =>
    {
        foreach (var cfg in new[] { "Debug", "Release" })
            DotNetTest(
                samplesProj.FullPath,
                new()
                {
                    MSBuildSettings = ConfigureMSBuild("samples", "test"),
                    Configuration = cfg,
                    NoBuild = true,
                });
    });

Task("test-samples-clean")
    .IsDependentOn("test-samples-restore")
    .Does(() =>
    {
        foreach (var cfg in new[] { "Debug", "Release" })
            DotNetClean(
                samplesProj.FullPath,
                new()
                {
                    MSBuildSettings = ConfigureMSBuild("samples", "clean"),
                    Configuration = cfg,
                });
    });

Task("test-samples")
    .IsDependentOn("test-samples-pack")
    .IsDependentOn("test-samples-publish")
    .IsDependentOn("test-samples-test")
    .IsDependentOn("test-samples-clean");

Task("test")
    .IsDependentOn("test-samples");

Task("upload-core-github")
    .WithCriteria(BuildSystem.GitHubActions.Environment.Workflow.Ref == "refs/heads/master")
    .IsDependentOn("pack-core")
    .Does(() =>
        DotNetTool(
            null,
            "gpr push",
            new ProcessArgumentBuilder()
                .AppendQuoted(githubGlob)
                .AppendSwitchQuotedSecret("-k", githubToken)));

Task("upload-core-nuget")
    .WithCriteria(BuildSystem.GitHubActions.Environment.Workflow.Ref.StartsWith("refs/tags/v"))
    .IsDependentOn("pack-core")
    .Does(() =>
    {
        foreach (var pkg in GetFiles(nugetGlob))
            DotNetNuGetPush(
                pkg,
                new()
                {
                    Source = "https://api.nuget.org/v3/index.json",
                    ApiKey = nugetToken,
                    SkipDuplicate = true,
                });
    });

RunTarget(target);
