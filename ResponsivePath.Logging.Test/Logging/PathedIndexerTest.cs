using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ResponsivePath.Logging
{
    [TestClass]
    public class PathedIndexerTest
    {
        [TestMethod]
        public void VariousPathIndexTest()
        {
            // Arrange
            var target = (ILogIndexer)new PathedIndexer(new System.Collections.Specialized.NameValueCollection
                {
                    { "Name", "Data.Profile.FirstName" },
                    { "Name", "Data.Profile.LastName" },
                    { "NoData", "Data.NotGiven" },
                    { "Other", "BadPath." },
                });
            var entry = new LogEntry
            {
                Data =
                {
                    { "Profile", new { FirstName = "Test", LastName = "Person" } }
                }
            };

            // Act
            target.IndexData(entry).Wait();

            // Assert
            Assert.IsTrue(entry.Indexes.GetValues("Name").Contains("Test"));
            Assert.IsTrue(entry.Indexes.GetValues("Name").Contains("Person"));
        }
    }
}
