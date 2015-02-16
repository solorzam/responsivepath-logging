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
    public class LoggerTest
    {
        [TestMethod]
        public void StandardTest()
        {
            // Arrange
            var mockAccumulator = new Mock<IDataAccumulator>();
            var mockIndexer = new Mock<ILogIndexer>();
            var mockRecorder = new Mock<ILogRecorder>();
            var config = new LogConfiguration()
            {
                Accumulators = { mockAccumulator.Object },
                Indexers = { mockIndexer.Object },
                Recorders = { mockRecorder.Object },
            };
            var target = (ILogger)new Logger(config);
            var entry = new LogEntry();

            // Act
            target.Record(entry).Wait();

            // Assert
            mockAccumulator.Verify(acc => acc.AccumulateData(entry), Times.Once());
            mockIndexer.Verify(acc => acc.IndexData(entry), Times.Once());
            mockRecorder.Verify(acc => acc.Save(entry), Times.Once());
        }

        [TestMethod]
        public void StandardWithExceptionsTest()
        {
            // Arrange
            var mockAccumulator = new Mock<IDataAccumulator>();
            var mockIndexer = new Mock<ILogIndexer>();
            var mockRecorder = new Mock<ILogRecorder>();
            var config = new LogConfiguration()
            {
                Accumulators = { mockAccumulator.Object },
                Indexers = { mockIndexer.Object },
                Recorders = { mockRecorder.Object },
            };
            var target = (ILogger)new Logger(config);
            var entry = new LogEntry();
            mockAccumulator.Setup(acc => acc.AccumulateData(entry)).Throws(new Exception());
            mockIndexer.Setup(acc => acc.IndexData(entry)).Throws(new Exception());
            mockRecorder.Setup(acc => acc.Save(entry)).Throws(new Exception());

            // Act
            target.Record(entry).Wait();

            // Assert
            mockAccumulator.Verify(acc => acc.AccumulateData(entry), Times.Once());
            mockIndexer.Verify(acc => acc.IndexData(entry), Times.Once());
            mockRecorder.Verify(acc => acc.Save(entry), Times.Once());
        }
    }
}
