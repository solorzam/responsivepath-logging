using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ResponsivePath.Logging
{
    [TestClass]
    public class LogExtensionsTest
    {
        [TestMethod]
        public void RecordNoExceptionTest1()
        {
            // Arrange
            Mock<ILogger> mockLogger = new Mock<ILogger>();

            // Act
            mockLogger.Object.Record("Extension test", Severity.Warning);

            // Assert
            mockLogger.Verify(l => l.Record(It.IsAny<LogEntry>()), Times.Once());
            mockLogger.Verify(l => l.Record(It.Is<LogEntry>(entry => entry.Message == "Extension test")), Times.Once());
            mockLogger.Verify(l => l.Record(It.Is<LogEntry>(entry => entry.Severity == Severity.Warning)), Times.Once());
            mockLogger.Verify(l => l.Record(It.Is<LogEntry>(entry => entry.Data.Count == 0)), Times.Once());
        }

        [TestMethod]
        public void RecordNoExceptionTest2()
        {
            // Arrange
            Mock<ILogger> mockLogger = new Mock<ILogger>();

            // Act
            mockLogger.Object.Record("Extension test", Severity.Warning, new Dictionary<string,object> { { "data", "found" } });

            // Assert
            mockLogger.Verify(l => l.Record(It.IsAny<LogEntry>()), Times.Once());
            mockLogger.Verify(l => l.Record(It.Is<LogEntry>(entry => entry.Message == "Extension test")), Times.Once());
            mockLogger.Verify(l => l.Record(It.Is<LogEntry>(entry => entry.Severity == Severity.Warning)), Times.Once());
            mockLogger.Verify(l => l.Record(It.Is<LogEntry>(entry => (string)entry.Data["data"] == "found")), Times.Once());
        }

        [TestMethod]
        public void RecordExceptionTest1()
        {
            // Arrange
            Mock<ILogger> mockLogger = new Mock<ILogger>();
            var ex = new Exception();

            // Act
            mockLogger.Object.Record("Extension test", ex);

            // Assert
            mockLogger.Verify(l => l.Record(It.IsAny<LogEntry>()), Times.Once());
            mockLogger.Verify(l => l.Record(It.Is<LogEntry>(entry => entry.Message == "Extension test")), Times.Once());
            mockLogger.Verify(l => l.Record(It.Is<LogEntry>(entry => entry.Severity == Severity.Error)), Times.Once());
            mockLogger.Verify(l => l.Record(It.Is<LogEntry>(entry => entry.Exception == ex)), Times.Once());
            mockLogger.Verify(l => l.Record(It.Is<LogEntry>(entry => entry.Data.Count == 0)), Times.Once());
        }

        [TestMethod]
        public void RecordExceptionTest2()
        {
            // Arrange
            Mock<ILogger> mockLogger = new Mock<ILogger>();
            var ex = new Exception();

            // Act
            mockLogger.Object.Record("Extension test", ex, Severity.Warning);

            // Assert
            mockLogger.Verify(l => l.Record(It.IsAny<LogEntry>()), Times.Once());
            mockLogger.Verify(l => l.Record(It.Is<LogEntry>(entry => entry.Message == "Extension test")), Times.Once());
            mockLogger.Verify(l => l.Record(It.Is<LogEntry>(entry => entry.Severity == Severity.Warning)), Times.Once());
            mockLogger.Verify(l => l.Record(It.Is<LogEntry>(entry => entry.Exception == ex)), Times.Once());
            mockLogger.Verify(l => l.Record(It.Is<LogEntry>(entry => entry.Data.Count == 0)), Times.Once());
        }

        [TestMethod]
        public void RecordExceptionTest3()
        {
            // Arrange
            Mock<ILogger> mockLogger = new Mock<ILogger>();
            var ex = new Exception();

            // Act
            mockLogger.Object.Record("Extension test", ex, Severity.Warning, new Dictionary<string, object> { { "data", "found" } });

            // Assert
            mockLogger.Verify(l => l.Record(It.IsAny<LogEntry>()), Times.Once());
            mockLogger.Verify(l => l.Record(It.Is<LogEntry>(entry => entry.Message == "Extension test")), Times.Once());
            mockLogger.Verify(l => l.Record(It.Is<LogEntry>(entry => entry.Severity == Severity.Warning)), Times.Once());
            mockLogger.Verify(l => l.Record(It.Is<LogEntry>(entry => entry.Exception == ex)), Times.Once());
            mockLogger.Verify(l => l.Record(It.Is<LogEntry>(entry => (string)entry.Data["data"] == "found")), Times.Once());
        }
    }
}
