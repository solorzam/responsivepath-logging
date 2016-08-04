using DeKreyConsulting.AdoTestability;
using DeKreyConsulting.AdoTestability.Testing.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResponsivePath.Logging
{
    using System.Threading;
    using CommandSetup = Dictionary<CommandBuilder, SetupCommandBuilderMock>;

    [TestClass]
    public class SqlLogRecorderTest
    {
        const string connectionString = "Server=.\\SQLExpress;Database=SqlLogDatabase;Trusted_Connection=True;";

        [TestMethod]
        public void ExplainCreateEntry()
        {
            SqlLogRecorder.cmdCreateEntry.ExplainSingleResult(BuildSqlConnection());
        }

        [TestMethod]
        public void ExplainCreateIndex()
        {
            SqlLogRecorder.cmdCreateIndex.ExplainSingleResult(BuildSqlConnection());
        }

        [TestMethod]
        public void SaveTest()
        {
            const int entryId = 18;
            var mocks = CommandBuilderMocks.SetupFor(new CommandSetup
            {
                { SqlLogRecorder.cmdCreateEntry, (mockCmd, record) => mockCmd.Setup(cmd => cmd.ExecuteScalarAsync(AnyCancellationToken)).ReturnsWithDelay(entryId).Callback(record) },
                { SqlLogRecorder.cmdCreateIndex, (mockCmd, record) => mockCmd.Setup(cmd => cmd.ExecuteNonQueryAsync(AnyCancellationToken)).ReturnsWithDelay(1).Callback(record) },
            });

            var target = (ILogRecorder)new SqlLogRecorder(connectionString, Severity.Debug, new string[0], mocks.ProviderFactory.Object);
            var log = new LogEntry
            {
                Timestamp = DateTime.Parse("2016-05-21"),
                Severity = Severity.Notice,
                Message = "Basic Test",
                Data = { { "Test", "Item" } },
                Indexes = { { "Index1", "Value" }, { "Index2", "AnotherValue" } }
            };

            target.Save(log).Wait();


            mocks.Connection.VerifySet(conn => conn.ConnectionString = connectionString);
            mocks.Commands[SqlLogRecorder.cmdCreateEntry].Verify(command => command.ExecuteScalarAsync(AnyCancellationToken), Times.Once());
            mocks.Commands[SqlLogRecorder.cmdCreateIndex].Verify(command => command.ExecuteNonQueryAsync(AnyCancellationToken), Times.Exactly(2));

            Assert.AreEqual(1, mocks.Executions[SqlLogRecorder.cmdCreateEntry].Count);
            Assert.AreEqual(mocks.Executions[SqlLogRecorder.cmdCreateEntry][0]["@timestamp"], log.Timestamp);
            Assert.AreEqual(mocks.Executions[SqlLogRecorder.cmdCreateEntry][0]["@severity"], log.Severity.ToString());
            Assert.AreEqual(mocks.Executions[SqlLogRecorder.cmdCreateEntry][0]["@message"], log.Message);
            Assert.AreEqual(mocks.Executions[SqlLogRecorder.cmdCreateEntry][0]["@data"], Newtonsoft.Json.JsonConvert.SerializeObject(log.Data));
            Assert.IsInstanceOfType(mocks.Executions[SqlLogRecorder.cmdCreateEntry][0]["@exception"], typeof(DBNull));

            Assert.AreEqual(2, mocks.Executions[SqlLogRecorder.cmdCreateIndex].Count);
            Assert.AreEqual(mocks.Executions[SqlLogRecorder.cmdCreateIndex][0]["@entryid"], entryId);
            Assert.AreEqual(mocks.Executions[SqlLogRecorder.cmdCreateIndex][0]["@key"], log.Indexes.AllKeys.First());
            Assert.AreEqual(mocks.Executions[SqlLogRecorder.cmdCreateIndex][0]["@value"], log.Indexes[log.Indexes.AllKeys.First()]);

            Assert.AreEqual(mocks.Executions[SqlLogRecorder.cmdCreateIndex][1]["@entryid"], entryId);
            Assert.AreEqual(mocks.Executions[SqlLogRecorder.cmdCreateIndex][1]["@key"], log.Indexes.AllKeys.Skip(1).First());
            Assert.AreEqual(mocks.Executions[SqlLogRecorder.cmdCreateIndex][1]["@value"], log.Indexes[log.Indexes.AllKeys.Skip(1).First()]);

        }

        [TestMethod]
        public void SaveExcludedTest()
        {
            const int entryId = 18;
            var mocks = CommandBuilderMocks.SetupFor(new CommandSetup
            {
                { SqlLogRecorder.cmdCreateEntry, (mockCmd, record) => mockCmd.Setup(cmd => cmd.ExecuteScalarAsync(AnyCancellationToken)).ReturnsWithDelay(entryId).Callback(record) },
                { SqlLogRecorder.cmdCreateIndex, (mockCmd, record) => mockCmd.Setup(cmd => cmd.ExecuteNonQueryAsync(AnyCancellationToken)).ReturnsWithDelay(1).Callback(record) },
            });

            var target = (ILogRecorder)new SqlLogRecorder(connectionString, Severity.Debug, new[] { "Test" }, mocks.ProviderFactory.Object);
            var log = new LogEntry
            {
                Timestamp = DateTime.Parse("2016-05-21"),
                Severity = Severity.Notice,
                Message = "Basic Test",
                Data = { { "Test", "Item" } },
                Indexes = { { "Index1", "Value" }, { "Index2", "AnotherValue" } }
            };

            target.Save(log).Wait();


            mocks.Connection.VerifySet(conn => conn.ConnectionString = connectionString);
            mocks.Commands[SqlLogRecorder.cmdCreateEntry].Verify(command => command.ExecuteScalarAsync(AnyCancellationToken), Times.Once());
            mocks.Commands[SqlLogRecorder.cmdCreateIndex].Verify(command => command.ExecuteNonQueryAsync(AnyCancellationToken), Times.Exactly(2));

            Assert.AreEqual(1, mocks.Executions[SqlLogRecorder.cmdCreateEntry].Count);
            Assert.AreEqual(mocks.Executions[SqlLogRecorder.cmdCreateEntry][0]["@timestamp"], log.Timestamp);
            Assert.AreEqual(mocks.Executions[SqlLogRecorder.cmdCreateEntry][0]["@severity"], log.Severity.ToString());
            Assert.AreEqual(mocks.Executions[SqlLogRecorder.cmdCreateEntry][0]["@message"], log.Message);
            Assert.AreEqual(mocks.Executions[SqlLogRecorder.cmdCreateEntry][0]["@data"], "{}");
            Assert.IsInstanceOfType(mocks.Executions[SqlLogRecorder.cmdCreateEntry][0]["@exception"], typeof(DBNull));

            Assert.AreEqual(2, mocks.Executions[SqlLogRecorder.cmdCreateIndex].Count);
            Assert.AreEqual(mocks.Executions[SqlLogRecorder.cmdCreateIndex][0]["@entryid"], entryId);
            Assert.AreEqual(mocks.Executions[SqlLogRecorder.cmdCreateIndex][0]["@key"], log.Indexes.AllKeys.First());
            Assert.AreEqual(mocks.Executions[SqlLogRecorder.cmdCreateIndex][0]["@value"], log.Indexes[log.Indexes.AllKeys.First()]);

            Assert.AreEqual(mocks.Executions[SqlLogRecorder.cmdCreateIndex][1]["@entryid"], entryId);
            Assert.AreEqual(mocks.Executions[SqlLogRecorder.cmdCreateIndex][1]["@key"], log.Indexes.AllKeys.Skip(1).First());
            Assert.AreEqual(mocks.Executions[SqlLogRecorder.cmdCreateIndex][1]["@value"], log.Indexes[log.Indexes.AllKeys.Skip(1).First()]);

        }

        private static CancellationToken AnyCancellationToken =>
            It.IsAny<CancellationToken>();

        private static SqlConnection BuildSqlConnection() =>
            (SqlConnection)SqlClientFactory.Instance.CreateConnection(connectionString);
    }
}
