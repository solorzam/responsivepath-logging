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
        public void StandardSynchronousTest()
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
                WaitForLogRecording = true,
            };
            var target = (ILogger)new Logger(config);
            var entry = new LogEntry();
            var mutex = new System.Threading.Mutex(true);
            mockIndexer.Setup(idx => idx.IndexData(entry)).Returns(Task.Run(() => mutex.WaitOne()));

            // Act
            target.Record(entry).Wait();

            // Assert
            mockAccumulator.Verify(acc => acc.AccumulateData(entry), Times.Once());
            mockIndexer.Verify(acc => acc.IndexData(entry), Times.Once());
            mockRecorder.Verify(acc => acc.Save(entry), Times.Once());
        }

        [TestMethod]
        public void StandardAsynchronousTest()
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
            var mutexIndex = new System.Threading.ManualResetEvent(false);
            var mutexIndexCompleted = new System.Threading.ManualResetEvent(false);
            var mutexRecord = new System.Threading.ManualResetEvent(false);
            mockIndexer.Setup(idx => idx.IndexData(entry)).Returns(Task.Run(() => { mutexIndex.WaitOne(); mutexIndexCompleted.Set(); }));
            mockRecorder.Setup(idx => idx.Save(entry)).Returns(() => Task.Run(() => { mutexIndexCompleted.WaitOne(); mutexRecord.Set(); }));

            // Act
            target.Record(entry).Wait();

            // Assert
            mockAccumulator.Verify(acc => acc.AccumulateData(entry), Times.Once());
            mockIndexer.Verify(acc => acc.IndexData(entry), Times.Once());
            mockRecorder.Verify(acc => acc.Save(entry), Times.Never());
            mutexIndex.Set();
            mutexRecord.WaitOne();
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
