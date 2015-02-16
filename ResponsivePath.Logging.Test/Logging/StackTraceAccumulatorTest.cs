using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ResponsivePath.Logging
{
    [TestClass]
    public class StackTraceAccumulatorTest
    {
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
            Assert.IsTrue(((string)logEntry.Data["StackTrace"]).StartsWith("   at ResponsivePath.Logging.StackTraceAccumulatorTest.AccumulateStackTraceExceptionTest() in \\ResponsivePath.Logging.Test\\Logging\\StackTraceAccumulatorTest.cs:line "));
            Assert.AreEqual("RWN+2uUbOWSYgNGRGAwjkA==", logEntry.Data["StackTraceHash"]);
        }
    }
}
