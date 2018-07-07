using System;
using FlubuCore.Context;
using FlubuCore.Scripting;

namespace BuildScript
{
    public class BuildScript : DefaultBuildScript
    {
        [FromArg("k", "Nuget key")] public string NugetKey { get; set; } = "Df";

        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
            context.Properties.Set(BuildProps.CompanyCopyright, "Copyright (C) 2010-2018 Flubu");
            context.Properties.Set(BuildProps.ProductId, "FlubuCore.Diff");
            context.Properties.Set(BuildProps.ProductName, "FlubuCore.Diff");
            context.Properties.Set(BuildProps.BuildDir, "output");
            context.Properties.Set(BuildProps.SolutionFileName, "FlubuCore.Diff.sln");
            context.Properties.Set(BuildProps.BuildConfiguration, "Release");
        }

        protected override void ConfigureTargets(ITaskContext context)
        {
            var buildVersion = context.CreateTarget("buildVersion")
                .SetAsHidden()
                .SetDescription("Fetches flubu version from FlubuCore.ProjectVersion.txt file.")
                .AddTask(x => x.FetchBuildVersionFromFileTask());

            var compile = context.CreateTarget("Compile")
                .AddCoreTask(x => x.UpdateNetCoreVersionTask("FlubuCore.Diff/FlubuCore.Diff.csproj"))
                .AddCoreTask(x => x.Build())
                .DependsOn(buildVersion);

            var publishNuget = context.CreateTarget("Publish.Nuget")
                .AddCoreTask(x => x.Pack()
                    .Project("FlubuCore.Diff/FlubuCore.Diff.csproj")
                    .IncludeSource()
                    .NoBuild())
                .AddCoreTask(x => x.NugetPush("FlubuCore.Diff/bin/release/FlubuCore.Diff.1.0.1.Nupkg")
                    .WithArguments("-s", "https://www.nuget.org/api/v2/package")
                    .WithArguments("-k", NugetKey)
                    .DoNotFailOnError());

            context.CreateTarget("Rebuild")
                .SetAsDefault()
                .DependsOn(compile, publishNuget);
        }
    }
}
