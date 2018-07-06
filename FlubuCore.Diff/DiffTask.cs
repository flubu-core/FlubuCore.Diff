using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks;

namespace FlubuCore.Diff
{
    public class DiffTask : TaskBase<string, DiffTask>
    {
        private readonly string _firstFilePathToCompare;

        private readonly string _secondFilePathToCompare;

        private readonly string _htmlReportOutputPath;

        private bool _noExportToFile;

        private bool _failOnDiff;

        private bool _failOnFileNotFound;

        /// <summary>
        /// Task compares 2 files and generates diff report in html.
        /// </summary>
        /// <param name="firstFilePathToCompare">path to the first file to compare</param>
        /// <param name="secondFilePathToCompare">path to the second file to compare</param>
        /// <param name="htmlReportOutputPath">path where html report will be stored.</param>
        public DiffTask(string firstFilePathToCompare, string secondFilePathToCompare, string htmlReportOutputPath)
        {
            _firstFilePathToCompare = firstFilePathToCompare;
            _secondFilePathToCompare = secondFilePathToCompare;
            _htmlReportOutputPath = htmlReportOutputPath;
        }

        protected override string Description { get; set; } = "Task compares 2 files and generates diff report in html.";

        /// <summary>
        /// If applied report is not saved to html.
        /// </summary>
        /// <returns></returns>
        public DiffTask NoExportToFile()
        {
            _noExportToFile = true;
            return this;
        }

        /// <summary>
        /// If applied task fails if there are any diffs in specified files.
        /// </summary>
        /// <returns></returns>
        public DiffTask FailOnDiff()
        {
            _failOnDiff = true;
            return this;
        }

        /// <summary>
        /// Task fails if any of specified files are not found.
        /// </summary>
        /// <returns></returns>
        public DiffTask FailOnFileNotFound()
        {
            _failOnFileNotFound = true;
            return this;
        }

        protected override string DoExecute(ITaskContextInternal context)
        {
            if (!File.Exists(_firstFilePathToCompare))
            {
                if (_failOnFileNotFound)
                {
                    throw new TaskExecutionException($"first file not found '{_firstFilePathToCompare}'.", 20);
                }

                context.LogInfo($"first file not found '{_firstFilePathToCompare}'. Skipping compare.");
                return null;
            }

            if (!File.Exists(_secondFilePathToCompare))
            {
                if (_failOnFileNotFound)
                {
                    throw new TaskExecutionException($"second file not found '{_secondFilePathToCompare}'.", 20);
                }

                context.LogInfo($"second file not found '{_secondFilePathToCompare}'. Skipping compare.");
                return null;
            }

            string newConfig = File.ReadAllText(_secondFilePathToCompare);

            var oldCOnf = File.ReadAllText(_firstFilePathToCompare);
            diff_match_patch dmp = new diff_match_patch();
            dmp.Diff_EditCost = 4;
            List<Diff> diff = dmp.diff_main(oldCOnf, newConfig);

            if (diff.Count == 1)
            {
                if (diff[0].operation == Operation.EQUAL)
                {
                    //// Files are the same.
                    return null;
                }
            }

            dmp.diff_cleanupSemantic(diff);

            var html = dmp.diff_prettyHtml(diff);

            if (!_noExportToFile)
            {
                File.WriteAllText(_htmlReportOutputPath, html);
            }

            if (_failOnDiff)
            {
                throw new TaskExecutionException($"File {_firstFilePathToCompare} and {_secondFilePathToCompare} are not the same", 21);
            }

            return html;
        }
    }
}
