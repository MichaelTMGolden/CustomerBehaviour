using CustomerBehaviour.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerBehaviour.Interfaces
{
    public interface IDataReader
    {
        public List<string> ReadCSVToString(string location);

        public List<RawCustomerData> GetCustomerRawData(string location);
    }
}
