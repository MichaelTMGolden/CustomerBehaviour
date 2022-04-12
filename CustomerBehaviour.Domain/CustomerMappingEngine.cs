using CustomerBehaviour.Definitions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomerBehaviour.Domain
{
    public class CustomerMappingEngine
    {
        public List<Customer> Run(string stringSnapshot)
        {
            var customersList = CreateCustomers(stringSnapshot);

            return customersList;
        }

        private List<Customer> CreateCustomers(string stringSnapshot)
        {
            List<Customer> customers = new List<Customer>();

            var customerLines = stringSnapshot.Split(new[] { '\n' });

            for (var i = 1; i < customerLines.Length; i++)
            {
                var customerInfo = customerLines[i].Split(',');
                var types = typeof(Customer);
                var properties = types.GetProperties();
                Customer customer = new Customer();
                var j = 0;
                foreach (PropertyInfo propertyInfo in properties)
                {
                    var value = getFloat(customerInfo[j]);
                    propertyInfo.SetValue(customer, value);
                    j++;
                }
                customers.Add(customer);
            }
            return customers;
        }

        private float getFloat(string info)
        {
            return float.Parse(info, NumberStyles.Float, CultureInfo.InvariantCulture); ;
        }
    }
}
