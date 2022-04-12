using CustomerBehaviour.Definitions;
using CustomerBehaviour.Domain;
using CustomerBehaviour.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace CustomerBehaviour.Application
{
    public class CustomerDataRetriever
    {
        private readonly IDataReader _reader;

        private readonly INormalizedDataWriter _writer;

        private readonly CustomerMappingEngine _mappingEngine;

        private readonly CustomerNormalizationEngine _normalizationEngine;

        //File directory
        private const string _dataLocation = @"C:\CustomerBehaviour\Analysis";
        public CustomerDataRetriever(IDataReader reader, INormalizedDataWriter writer, CustomerMappingEngine mappingEngine, CustomerNormalizationEngine normalizationEngine)
        {
            _reader = reader;

            _writer = writer;

            _mappingEngine = mappingEngine;

            _normalizationEngine = normalizationEngine;
        }

        public List<CustomersBehaviourSnapshot> GetCustomersBehaviorSnapshots()
        {
            List<CustomersBehaviourSnapshot> snapshots = new List<CustomersBehaviourSnapshot>();

            var rawDataFiles = _reader.GetCustomerRawData(_dataLocation);

            foreach (var rawDataFile in rawDataFiles)
            {
                var customers = CreateSnapshot(rawDataFile.rawData);
                var snapshot = NormalizeCustomers(customers, rawDataFile.name);
                _writer.WriteNormalizedDataToFile(snapshot);
                snapshots.Add(snapshot);
            }
            return snapshots;
        }

        private CustomersBehaviourSnapshot NormalizeCustomers(List<Customer> customers, string fileName)
        {
            var cleanName = CleanName(fileName);
            var normalizedCustomer = _normalizationEngine.Run(customers);
            var snapshot = new CustomersBehaviourSnapshot()
            {
                name = cleanName,
                customers = normalizedCustomer
            };
            return snapshot;
        }

        private string CleanName(string fileName)
        {
            var stringArr = fileName.Split('\\');
            return stringArr.Last().Trim();
        }

        private List<Customer> CreateSnapshot(string stringSnapshot)
        {
            var snapshot = _mappingEngine.Run(stringSnapshot);
            return snapshot;
        }
    }
}
