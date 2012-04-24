using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;

namespace MSBuildVersioning
{
    /// <summary>
    /// Abstract class providing helper methods to execute and scrape information from source
    /// control command line programs.
    /// </summary>
    public abstract class SourceControlInfoProvider
    {
        protected SourceControlInfoProvider()
        {
            Path = "";
        }

        public virtual string Path { get; set; }

        public abstract string SourceControlName { get; }

        protected virtual IList<string> ExecuteCommand(string fileName, string arguments)
        {
            return ExecuteCommand(fileName, arguments, null);
        }

        protected virtual IList<string> ExecuteCommand(string fileName, string arguments, Func<int, string, bool> errorHandler)
        {
            IList<string> output = new List<string>();
            ExecuteCommand(fileName, arguments, outputLine => output.Add(outputLine), errorHandler);
            return output;
        }

        protected virtual void ExecuteCommand(string fileName, string arguments,
            Action<string> outputHandler, Func<int, string, bool> errorHandler)
        {
            StringBuilder error = new StringBuilder();

            using (Process process = new Process())
            {
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory();
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.OutputDataReceived += (o, e) =>
                    {
                        if (e.Data != null)
                            outputHandler(e.Data);
                    };

                process.ErrorDataReceived += (o, e) =>
                    {
                        if (e.Data != null)
                            error.AppendLine(e.Data);
                    };

                try
                {
                    process.Start();
                }
                catch (Win32Exception e)
                {
                    if (e.NativeErrorCode == 2) // file not found
                    {
                        throw new BuildErrorException(String.Format(
                            "{0} command \"{1}\" could not be found." + Environment.NewLine +
                            "Please ensure that {0} is installed.",
                            SourceControlName, fileName));
                    }
                    else
                    {
                        throw;
                    }
                }

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                var reportError = errorHandler != null && errorHandler(process.ExitCode, error.ToString());
                if (reportError && (process.ExitCode != 0 || error.Length > 0))
                {
                    throw new BuildErrorException(String.Format(
                        "{0} command \"{1} {2}\" exited with code {3}.\n{4}",
                        SourceControlName, fileName, arguments, process.ExitCode, error));
                }
            }
        }
    }
}
