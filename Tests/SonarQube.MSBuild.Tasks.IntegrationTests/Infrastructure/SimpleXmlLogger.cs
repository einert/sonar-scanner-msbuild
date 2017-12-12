using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using SonarQube.MSBuild.Tasks.IntegrationTests;


// See MSDN for an example: https://msdn.microsoft.com/en-us/library/ms171471#Anchor_5

// Example usage: msbuild x.csproj /logger:Logger.SimpleFileLogger,..\Logger\bin\Debug\Logger.dll;log1.txt /noconsolelogger

namespace Logger
{    public class SimpleXmlLogger : ILogger
    {
        private IEventSource eventSource;

        private BuildLog log;

        private string fileName;

        public LoggerVerbosity Verbosity { get; set; }

        public string Parameters { get; set; }

        public void Initialize(IEventSource eventSource)
        {
            // The name of the log file should be passed as the first item in the
            // "parameters" specification in the /logger switch.  It is required
            // to pass a log file to this logger. Other loggers may have zero or more than 
            // one parameters.
            if (null == Parameters)
            {
                throw new LoggerException("Log file was not set.");
            }
            string[] parameters = Parameters.Split(';');

            string logFile = parameters[0];
            if (String.IsNullOrEmpty(logFile))
            {
                throw new LoggerException("Log file was not set.");
            }

            if (parameters.Length > 1)
            {
                throw new LoggerException("Too many parameters passed.");
            }

            this.fileName = logFile;

            this.eventSource = eventSource;

            this.eventSource.BuildFinished += EventSource_BuildFinished;
            this.eventSource.BuildStarted += EventSource_BuildStarted;

            eventSource.TargetStarted += EventSource_TargetStarted;
            eventSource.TaskStarted += EventSource_TaskStarted;

            eventSource.WarningRaised += EventSource_WarningRaised;
            eventSource.ErrorRaised += EventSource_ErrorRaised;

            this.log = new BuildLog();
        }

        private void EventSource_BuildStarted(object sender, BuildStartedEventArgs e)
        {
            foreach(KeyValuePair<string, string> kvp in e.BuildEnvironment)
            {
                this.log.BuildProperties.Add(new BuildProperty() { Name = kvp.Key, Value = kvp.Value });
            }
        }

        private void EventSource_BuildFinished(object sender, BuildFinishedEventArgs e)
        {
            this.log.BuildSucceeded = e.Succeeded;
        }

        private void EventSource_TaskStarted(object sender, TaskStartedEventArgs e)
        {
            this.log.Tasks.Add(e.TaskName);
        }

        private void EventSource_TargetStarted(object sender, TargetStartedEventArgs e)
        {
            this.log.Targets.Add(e.TargetName);
        }
        private void EventSource_ErrorRaised(object sender, BuildErrorEventArgs e)
        {
            log.Errors.Add(e.Message);
        }

        private void EventSource_WarningRaised(object sender, BuildWarningEventArgs e)
        {
            log.Warnings.Add(e.Message);
        }

        public void Shutdown()
        {
            this.log.Save(this.fileName);
        }
    }
}
