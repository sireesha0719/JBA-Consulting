using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace JBA
{

    public class DataProcessor
    {
        private string connectionString;

        public DataProcessor(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void ProcessData(string fileName)
        {
            var data = ReadDataFromFile(fileName);
            CreateDatabaseTable();
            InsertDataIntoDatabase(data);
        }

        public List<PrecipitationData> ReadDataFromFile(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<PrecipitationDataMapper>();
                return csv.GetRecords<PrecipitationData>().ToList();
            }
        }

        public void CreateDatabaseTable()
        {
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                string createTableQuery = @"
                    IF OBJECT_ID('PrecipitationData', 'U') IS NULL
                    BEGIN
                        CREATE TABLE PrecipitationData
                        (
                            Xref INT,
                            Yref INT,
                            Date DATE,
                            Value INT
                        )
                    END";
                connection.Execute(createTableQuery);
            }
        }

        public void InsertDataIntoDatabase(List<PrecipitationData> data)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string insertQuery = "INSERT INTO PrecipitationData (Xref, Yref, Date, Value) VALUES (@Xref, @Yref, @Date, @Value)";
                connection.Execute(insertQuery, data);
            }
        }
    }
}

  
