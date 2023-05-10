using Dapper;
using JBA;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace JBAUnitTest
{
    public class Tests
    {
       // private readonly string ConnectionString = ConfigurationManager.ConnectionStrings["PrecipitationDbConnectionString"].ConnectionString; // Replace with your database connection string
        private readonly string TestDataFilePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"cru-ts-2-10.1991-2000-cutdown.pre"); 

        private DataProcessor dataProcessor;
        private SqlConnection connection;
        private string connectionString;



        [SetUp]
        public void SetUp()
        {
            var configMap = new ExeConfigurationFileMap { ExeConfigFilename = "path/App.Config" };
            var config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            connectionString = config.ConnectionStrings.ConnectionStrings["PrecipitationDbConnectionString"].ConnectionString;
            connection = new SqlConnection(connectionString);
            connection.Open();
            dataProcessor = new DataProcessor(connectionString);
        }

        [TearDown]
        public void TearDown()
        {
            connection.Close();
        }

        [Test]
        public void ProcessData_ValidFile_SuccessfullyInsertsDataIntoDatabase()
        {
            // Arrange

            // Act
            dataProcessor.ProcessData(TestDataFilePath);

            // Assert
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT COUNT(*) FROM PrecipitationData";
                var result = connection.ExecuteScalar<int>(selectQuery);
                Assert.AreEqual(12, result); // Assuming the test data file contains 12 records
            }
        }

        [Test]
        public void ReadDataFromFile_ValidFile_ReturnsCorrectDataList()
        {
            // Arrange

            // Act
            var result = dataProcessor.ReadDataFromFile(TestDataFilePath);

            // Assert
            Assert.AreEqual(12, result.Count); // Assuming the test data file contains 12 records

            // Validate specific data values
            Assert.AreEqual(1, result[0].Xref);
            Assert.AreEqual(148, result[0].Yref);
            Assert.AreEqual(new DateTime(1991, 1, 1), result[0].Date);
            Assert.AreEqual(3020, result[0].Value);

            Assert.AreEqual(1, result[11].Xref);
            Assert.AreEqual(311, result[11].Yref);
            Assert.AreEqual(new DateTime(1991, 12, 31), result[11].Date);
            Assert.AreEqual(450, result[11].Value);
        }

        [Test]
        public void CreateDatabaseTable_TableDoesNotExist_CreatesTableSuccessfully()
        {
            // Arrange

            // Act
            dataProcessor.CreateDatabaseTable();

            // Assert
            using (var connection = new SqlConnection( connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT COUNT(*) FROM sys.tables WHERE name = 'PrecipitationData'";
                var result = connection.ExecuteScalar<int>(selectQuery);
                Assert.AreEqual(1, result);
            }
        }

        [Test]
        public void InsertDataIntoDatabase_ValidData_InsertsDataSuccessfully()
        {
            // Arrange
            var testData = new List<PrecipitationData>
            {
                new PrecipitationData { Xref = 1, Yref = 148, Date = new DateTime(1991, 1, 1), Value = 3020 },
                new PrecipitationData { Xref = 1, Yref = 148, Date = new DateTime(1991, 1, 2), Value = 2820 },
                // Add more test data as needed
            };

            // Act
            dataProcessor.InsertDataIntoDatabase(testData);

            // Assert
            using (var connection = new SqlConnection( connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT COUNT(*) FROM PrecipitationData";
                var result = connection.ExecuteScalar<int>(selectQuery);
                Assert.AreEqual(testData.Count, result);
            }
        }


        private List<PrecipitationData> GetExpectedData()
        {
            // Define the expected data manually based on the provided file contents
            List<PrecipitationData> expectedData = new List<PrecipitationData>();

            // Add data points to the list
            // Replace this section with the actual data from the file
            // Example:
            expectedData.Add(new PrecipitationData { Xref = 1, Yref = 148, Date = new DateTime(2004, 1, 22), Value = 3020 });
            expectedData.Add(new PrecipitationData { Xref = 1, Yref = 148, Date = new DateTime(2004, 1, 23), Value = 2820 });
            // Add more data points...

            return expectedData;
        }

     
    }
}
