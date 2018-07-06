using System;
using FlubuCore.Context;
using FlubuCore.Scripting;

namespace BuildScript
{
    public class BuildScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
           
            context.Properties.Set(BuildProps.CompanyCopyright, "Copyright (C) 2010-2018 Flubu");
            context.Properties.Set(BuildProps.ProductId, "FlubuCore");
            context.Properties.Set(BuildProps.ProductName, "FlubuCore");
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
                .AddCoreTask(x => x.Pack().IncludeSource().NoBuild())
                .AddCoreTask(x => x.NugetPush("FlubuCore.Diff/bin/release/net.standard1.6/FlubuCore.Diff.Nupkg"));

            context.CreateTarget("Rebuild")
                .DependsOn(compile, publishNuget);

        }
    }
}
