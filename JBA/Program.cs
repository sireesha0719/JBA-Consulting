using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace JBA
{
    public class Program
    {
        static void Main(string[] args)
        {

            //Read the file name from command line argument
            string path = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"cru-ts-2-10.1991-2000-cutdown.pre");


            string connectionString = ConfigurationManager.ConnectionStrings["PrecipitationDbConnectionString"].ConnectionString;  // Replace with your database connection string
            string fileName = path; // Replace with the path to your data file

            var dataProcessor = new DataProcessor(connectionString);
            dataProcessor.ProcessData(fileName);

        }

        


    }
}
