using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Diff;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    /// <summary>
    /// Task compares 2 files and generates diff report in html.
    /// </summary>
    /// <param name="firstFilePathToCompare">path to the first file to compare</param>
    /// <param name="secondFilePathToCompare">path to the second file to compare</param>
    /// <param name="htmlReportOutputPath">path where html report will be stored.</param>
    public static class TaskFluentInterfaceExtensions
    {
           public static DiffTask DiffTask(this ITaskFluentInterface flubu, string firstFilePathToCompare, string secondFilePathToCompare, string htmlReportOutputPath)
        {
            return new DiffTask(firstFilePathToCompare, secondFilePathToCompare, htmlReportOutputPath);
        }
    }
}
