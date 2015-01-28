using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ResponsivePath.Logging
{
    /// <summary>
    /// A log recorder that writes to a sql database.
    /// </summary>
    /// <remarks>
    /// Commands in the log recorder expect the connection string to default to a database with the following tables.
    /// 
    /// CREATE TABLE [dbo].[Entries](
    /// 	[EntityId] [int] IDENTITY(1,1) NOT NULL,
    /// 	[Timestamp] [datetime2](7) NOT NULL,
    /// 	[Severity] [varchar](50) NOT NULL,
    /// 	[Message] [varchar](200) NULL,
    /// 	[Exception] [varchar](max) NULL,
    /// 	[Data] [varchar](max) NULL,
    ///  CONSTRAINT [PK_Entries] PRIMARY KEY CLUSTERED 
    /// (
    /// 	[EntityId] ASC
    /// )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    /// ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
    /// 
    /// 
    /// CREATE TABLE [dbo].[EntryIndexes](
    /// 	[EntryId] [int] NOT NULL,
    /// 	[Key] [varchar](50) NOT NULL,
    /// 	[Value] [varchar](200) NOT NULL,
    ///  CONSTRAINT [PK_EntryIndexes] PRIMARY KEY CLUSTERED 
    /// (
    /// 	[EntryId] ASC,
    /// 	[Key] ASC,
    /// 	[Value] ASC
    /// )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    /// ) ON [PRIMARY]
    /// 
    /// ALTER TABLE [dbo].[EntryIndexes]  WITH CHECK ADD  CONSTRAINT [FK_EntryIndexes_Entries] FOREIGN KEY([EntryId])
    /// REFERENCES [dbo].[Entries] ([EntityId])
    /// 
    /// ALTER TABLE [dbo].[EntryIndexes] CHECK CONSTRAINT [FK_EntryIndexes_Entries]
    /// </remarks>
    public class SqlLogRecorder : ILogRecorder
    {
        private readonly string connectionString;
        private readonly Severity minSeverity;
        private readonly IList<string> propertiesToIgnore;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connectionString">Specifies the connection string for the sql connection</param>
        /// <param name="minSeverity">The minimum severity to log to the database.</param>
        /// <param name="propertiesToIgnore">Properties in data to ignore. Useful for not logging SSNs, card numbers, and other sensitive 
        /// information that might be nested in logged objects.</param>
        public SqlLogRecorder(string connectionString, Severity minSeverity, IEnumerable<string> propertiesToIgnore)
        {
            this.connectionString = connectionString;
            this.minSeverity = minSeverity;
            this.propertiesToIgnore = propertiesToIgnore.ToList().AsReadOnly();
        }

        async Task ILogRecorder.Save(LogEntry logEntry)
        {
            if (logEntry.Severity < minSeverity)
            {
                return;
            }

            using (var connection = new SqlConnection(connectionString))
            using (var cmdCreateEntry = new SqlCommand(@"
INSERT INTO Entries (Timestamp, Severity, Message, Exception, Data)
VALUES (@timestamp, @severity, @message, @exception, @data);
SELECT SCOPE_IDENTITY();
", connection)
 {
     Parameters = 
     { 
        new SqlParameter("@timestamp", logEntry.Timestamp), 
        new SqlParameter("@severity", logEntry.Severity.ToString("g")),
        new SqlParameter("@message", logEntry.Message ?? (object)DBNull.Value),
        new SqlParameter("@exception", JsonEncode(logEntry.Exception) ?? (object)DBNull.Value),
        new SqlParameter("@data", JsonEncode(logEntry.Data) ?? (object)DBNull.Value),
     }
 })
            using (var cmdCreateIndex = new SqlCommand(@"
INSERT INTO EntryIndexes ([EntryId], [Key], [Value])
VALUES (@entryid, @key, @value);
", connection)
 {
     Parameters = 
     { 
         new SqlParameter("@entryid", null),
         new SqlParameter("@key", null),
         new SqlParameter("@value", null),
     }
 })
            {
                await connection.OpenAsync();
                var entryId = Convert.ToInt32(await cmdCreateEntry.ExecuteScalarAsync());

                cmdCreateIndex.Parameters["@entryid"].Value = entryId;
                foreach (var index in from key in logEntry.Indexes.AllKeys
                                      from value in logEntry.Indexes.GetValues(key)
                                      select new { key, value })
                {
                    cmdCreateIndex.Parameters["@key"].Value = index.key;
                    cmdCreateIndex.Parameters["@value"].Value = index.value;

                    await cmdCreateIndex.ExecuteNonQueryAsync();
                }
            }
        }

        private string JsonEncode(object data)
        {
            if (data == null)
                return null;
            return JsonConvert.SerializeObject(data, new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new HidePropertiesContractResolver(propertiesToIgnore)
            });
        }

        private class HidePropertiesContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
        {
            private IList<string> _propertiesToIgnore = null;

            public HidePropertiesContractResolver(IList<string> propertiesToIgnore)
            {
                _propertiesToIgnore = propertiesToIgnore;
            }

            protected override IList<JsonProperty> CreateProperties(Type type, Newtonsoft.Json.MemberSerialization memberSerialization)
            {
                IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
                if (_propertiesToIgnore != null)
                {
                    return properties.Where(p => !_propertiesToIgnore.Contains(p.PropertyName, StringComparer.InvariantCultureIgnoreCase)).ToList();
                }
                else
                {
                    return properties;
                }
            }
        }
    }
}
