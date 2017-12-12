using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using SonarQube.Common;

namespace SonarQube.MSBuild.Tasks.IntegrationTests
{
    public static class BuildRunner
    {
        public static BuildLog BuildTargets(string projectFile, params string[] targets)
        {
            // TODO: calculate exe path
            var exePath = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\msbuild.exe";

            var logPath = projectFile + ".log";

            var msbuildArgs = new List<string>();
            var loggerType = typeof(Logger.SimpleXmlLogger);
            msbuildArgs.Add($"/logger:{loggerType.FullName},{loggerType.Assembly.Location};{logPath}");
            msbuildArgs.Add($"/t:" + string.Join(";", targets.ToArray()));
            msbuildArgs.Add(projectFile);

            var args = new ProcessRunnerArguments(exePath, false, new ConsoleLogger(true));
            args.CmdLineArgs = msbuildArgs;
            var runner = new ProcessRunner();
            var success = runner.Execute(args);

            success.Should().BeTrue();
            File.Exists(logPath).Should().BeTrue();

            return BuildLog.Load(logPath);
        }
         
    }
}
