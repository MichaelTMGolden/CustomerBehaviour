using CustomerBehaviour.Definitions;
using CustomerBehaviour.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace CustomerBehaviour.Application
{
    public class DataReader : IDataReader
    {
        public List<string> ReadCSVToString(string location)
        {
            List<string> csvFiles = new List<string>();

            foreach (string fileName in Directory.GetFiles(location))
            {
                csvFiles.Add(File.ReadAllText(fileName));
            }
            return csvFiles;
        }
        public List<RawCustomerData> GetCustomerRawData(string location)
        {
            var rawObjects = new List<RawCustomerData>();

            foreach (string fileName in Directory.GetFiles(location))
            {
                var customerDate = new RawCustomerData
                {
                    name = fileName,
                    rawData = File.ReadAllText(fileName)
                };
                rawObjects.Add(customerDate);
            }
            return rawObjects;
        }
        
    }
}
