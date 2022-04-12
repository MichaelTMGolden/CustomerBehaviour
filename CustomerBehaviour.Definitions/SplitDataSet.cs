using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerBehaviour.Definitions
{
    public class SplitDataSet
    {
        public List<Customer> trainningSet { get; set; }
        public List<Customer> testSet { get; set; }
    }
}
