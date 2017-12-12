using System;
using System.Linq;
using Microsoft.Build.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SonarQube.MSBuild.Tasks.IntegrationTests
{
    internal static class BuildLoggerAssertions
    {
        /// <summary>
        /// Checks that building the specified target succeeded.
        /// </summary>
        public static void AssertTargetSucceeded(this BuildLog log, string target)
        {
            AssertTargetExecuted(log, target);
            Assert.IsTrue(log.BuildSucceeded);
        }

        /// <summary>
        /// Checks that building the specified target failed.
        /// </summary>
        public static void AssertTargetFailed(this BuildLog log, string target)
        {
            AssertTargetExecuted(log, target);
            Assert.IsFalse(log.BuildSucceeded);
        }

        public static void AssertTargetExecuted(this BuildLog log, string targetName)
        {
            var found = log.Targets.FirstOrDefault(t => t.Equals(targetName, StringComparison.InvariantCulture));
            Assert.IsNotNull(found, "Specified target was not executed: {0}", targetName);
        }

        public static void AssertTargetNotExecuted(this BuildLog log, string targetName)
        {
            var found = log.Targets.FirstOrDefault(t => t.Equals(targetName, StringComparison.InvariantCulture));
            Assert.IsNull(found, "Not expecting the target to have been executed: {0}", targetName);
        }

        public static void AssertTaskExecuted(this BuildLog log, string taskName)
        {
            var found = log.Tasks.FirstOrDefault(t => t.Equals(taskName, StringComparison.InvariantCulture));
            Assert.IsNotNull(found, "Specified task was not executed: {0}", taskName);
        }

        public static void AssertTaskNotExecuted(this BuildLog log, string taskName)
        {
            var found = log.Tasks.FirstOrDefault(t => t.Equals(taskName, StringComparison.InvariantCulture));
            Assert.IsNull(found, "Not expecting the task to have been executed: {0}", taskName);
        }

        /// <summary>
        /// Checks that the expected tasks were executed in the specified order
        /// </summary>
        public static void AssertExpectedTargetOrdering(this BuildLog log, params string[] expected)
        {
            foreach (var target in expected)
            {
                AssertTargetExecuted(log,target);
            }

            var actual = log.Targets.Where(t => expected.Contains(t, StringComparer.Ordinal)).ToArray();

            Console.WriteLine("Expected target order: {0}", string.Join(", ", expected));
            Console.WriteLine("Actual target order: {0}", string.Join(", ", actual));

            CollectionAssert.AreEqual(expected, actual, "Targets were not executed in the expected order");
        }

        public static void AssertNoWarningsOrErrors(this BuildLog log)
        {
            AssertExpectedErrorCount(log, 0);
            AssertExpectedWarningCount(log, 0);
        }

        public static void AssertExpectedErrorCount(this BuildLog log, int expected)
        {
            Assert.AreEqual(expected, log.Errors.Count, "Unexpected number of errors raised");
        }

        public static void AssertExpectedWarningCount(this BuildLog log, int expected)
        {
            Assert.AreEqual(expected, log.Warnings.Count, "Unexpected number of warnings raised");
        }
    }
}
