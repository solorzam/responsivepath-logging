using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace ResponsivePath.Logging
{
    [TestClass]
    public class StackTraceAccumulatorTest
    {
        static readonly Regex regex = new Regex("^   at ResponsivePath\\.Logging\\.StackTraceAccumulatorTest\\..+ in \\\\ResponsivePath\\.Logging\\.Test\\\\Logging\\\\StackTraceAccumulatorTest\\.cs\\:line ");

        [TestMethod]
        public void AccumulateStackTraceExceptionTest()
        {
            // Arrange
            var exception = new Exception();
            try 
            { 
                throw exception; 
            }
            catch { }
            var target = (IDataAccumulator)new StackTraceAccumulator(0, new[] { @" in (.*)\\ResponsivePath" });
            var logEntry = new LogEntry { Exception = exception };

            // Act
            target.AccumulateData(logEntry);

            // Assert
            Assert.IsTrue(regex.IsMatch(((string)logEntry.Data["StackTrace"])));
            Assert.IsNotNull(logEntry.Data["StackTraceHash"]);
        }

        [TestMethod]
        public async Task AccumulateStackTraceExceptionTaskTest()
        {
            // Arrange
            var exception = new Exception();
            Func<Task> action = async () =>
            {
                await Task.Yield();
                throw exception;
            };
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            var target = (IDataAccumulator)new StackTraceAccumulator(0, new[] { @" in (.*)\\ResponsivePath" });
            var logEntry = new LogEntry { Exception = exception };

            // Act
            target.AccumulateData(logEntry);

            // Assert
            Assert.IsTrue(((string)logEntry.Data["StackTrace"]).Contains("--- Async ---"));
            Assert.IsTrue(regex.IsMatch(((string)logEntry.Data["StackTrace"])));
            Assert.IsNotNull(logEntry.Data["StackTraceHash"]);
        }


        [TestMethod]
        public void AccumulateStackTraceTest()
        {
            // Arrange
            var target = (IDataAccumulator)new StackTraceAccumulator(0, new[] { @" in (.*)\\ResponsivePath" });
            var logEntry = new LogEntry { };

            // Act
            target.AccumulateData(logEntry);

            // Assert
            Assert.IsTrue(regex.IsMatch(((string)logEntry.Data["StackTrace"])));
            Assert.IsNotNull(logEntry.Data["StackTraceHash"]);
        }

        [TestMethod]
        public async Task AccumulateStackTraceTaskTest()
        {
            // Arrange
            var target = (IDataAccumulator)new StackTraceAccumulator(0, new[] { @" in (.*)\\ResponsivePath" });
            var logEntry = new LogEntry { };
            Func<Task> action = async () =>
            {
                await Task.Yield();
                target.AccumulateData(logEntry);
            };

            // Act
            await action();

            // Assert
            Assert.IsTrue(((string)logEntry.Data["StackTrace"]).Contains("--- Async ---"));
            Assert.IsTrue(regex.IsMatch(((string)logEntry.Data["StackTrace"])));
            Assert.IsNotNull(logEntry.Data["StackTraceHash"]);
        }
    }
}
