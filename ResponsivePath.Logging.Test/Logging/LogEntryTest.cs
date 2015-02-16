using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ResponsivePath.Logging
{
    [TestClass]
    public class LogEntryTest
    {
        [TestMethod]
        public void LogEntryConstructorTest()
        {
            // Arrange
            var startTime = DateTimeOffset.Now;

            // Act
            var result = new LogEntry();

            // Assert
            Assert.IsTrue(startTime <= result.Timestamp && result.Timestamp <= DateTimeOffset.Now);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(0, result.Data.Count);
            Assert.IsNotNull(result.Indexes);
            Assert.AreEqual(0, result.Indexes.Count);
        }
    }
}
