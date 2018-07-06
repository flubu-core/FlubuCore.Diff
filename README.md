FlubuCore.Diff is a [FlubuCore](https://github.com/flubu-core/flubu.core) plugin. It adds Diff task to task fluent interface. Diff task compares 2 specified files and generates html report with differences.

        protected override void ConfigureTargets(ITaskContext context)
        {
               context.Tasks().DiffTask("FirstFile.config", "SecondFile.config", "Diff.html");
        }
