using CustomerBehaviour.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CustomerBehaviour.Domain
{
    public class CustomerNormalizationEngine
    {
        public List<Customer> Run(List<Customer> customers)
        {
            var phantomCustomers = GetMedianCustomer(customers);
            var stdCustomer = GetStandardDeviationCustomer(phantomCustomers);
            var customerList = new List<Customer>();

            foreach (var customer in customers)
            {
                var normalizedCustomer = NormalizeCustomer(customer, phantomCustomers[0], stdCustomer);
                customerList.Add(normalizedCustomer);
            }
            return customerList;
        }

        private Customer GetStandardDeviationCustomer(List<Customer> phantomCustomers)
        {
            var types = typeof(Customer);
            var properties = types.GetProperties();
            var stdCustomer = new Customer();

            foreach (PropertyInfo propertyInfo in properties)
            {
                var mediancustomers = phantomCustomers[0];
                var lowPercentileCustomers = phantomCustomers[1];
                var highPercentileCustomers = phantomCustomers[2];

                var std = (((float)propertyInfo.GetValue(mediancustomers) - (float)propertyInfo.GetValue(lowPercentileCustomers)) + 
                    ((float)propertyInfo.GetValue(highPercentileCustomers) - (float)propertyInfo.GetValue(mediancustomers)))/2;

                propertyInfo.SetValue(stdCustomer, std);
            }
            return stdCustomer;
        }

        private List<Customer> GetMedianCustomer(List<Customer> customers)
        {
            var types = typeof(Customer);
            var properties = types.GetProperties();
            var customerList = new List<Customer>();
            Customer medianCustomer = new Customer();
            Customer lowPercentileCustomer = new Customer();
            Customer highPercentileCustomer = new Customer();
            var fieldList = new List<float>();
            foreach (PropertyInfo propertyInfo in properties)
            { 
                foreach(Customer customer in customers)
                {
                    fieldList.Add((float)propertyInfo.GetValue(customer));
                }
                var lowPercentile = GetPercentile(16, fieldList);
                propertyInfo.SetValue(lowPercentileCustomer, lowPercentile);

                var highPercentile = GetPercentile(84, fieldList);
                propertyInfo.SetValue(highPercentileCustomer, highPercentile);

                var median = findMedian(fieldList);
                propertyInfo.SetValue(medianCustomer, median);
            }
            
            customerList.Add(medianCustomer);
            customerList.Add(lowPercentileCustomer);
            customerList.Add(highPercentileCustomer);
            return customerList;
        }

        private float GetPercentile(int percentile, List<float> fieldList)
        {
            var sortedNumbers = fieldList.OrderBy(n => n);
            var ordinal = Convert.ToInt32((percentile/100m)*fieldList.Count());
            return sortedNumbers.ElementAt(ordinal);
        }

        private float findMedian(List<float> fieldList)
        {
            int numberCount = fieldList.Count();
            int halfIndex = fieldList.Count() / 2;
            var sortedNumbers = fieldList.OrderBy(n => n);
            float median;
            if ((numberCount % 2) == 0)
            {
                median = ((sortedNumbers.ElementAt(halfIndex) + sortedNumbers.ElementAt((halfIndex - 1))) / 2);
            }
            else
            {
                median = sortedNumbers.ElementAt(halfIndex);
            }
            return median;
        }

        private Customer GetMinCustomer(List<Customer> customers)
        {
            Console.WriteLine("Creating Min Customer");
            var types = typeof(Customer);
            var properties = types.GetProperties();
            Customer customerMinimums = new Customer();

            foreach (PropertyInfo propertyInfo in properties)
            {
                var min = customers.Min(customer => propertyInfo.GetValue(customer));
                propertyInfo.SetValue(customerMinimums, min);
            }

            return customerMinimums;
        }

        private Customer GetMaxCustomer(List<Customer> customers)
        {
            Console.WriteLine("Creating Max Customer");
            var types = typeof(Customer);
            var properties = types.GetProperties();
            Customer customerMaximus = new Customer();

            foreach (PropertyInfo propertyInfo in properties)
            {
                var max = customers.Max(customer => propertyInfo.GetValue(customer));
                propertyInfo.SetValue(customerMaximus, max);
            }

            return customerMaximus;
        }

        private Customer NormalizeCustomer(Customer customer, Customer medianCustomer, Customer stdCustomer)
        {
            Console.WriteLine("Normalizing Customer" + customer.target.ToString());
            var types = typeof(Customer);
            var properties = types.GetProperties();
            Customer normalizedCustomer = new Customer();

            foreach (PropertyInfo propertyInfo in properties)
            {
                var value = NormalizeDistibution((float)propertyInfo.GetValue(customer), (float)propertyInfo.GetValue(medianCustomer), (float)propertyInfo.GetValue(stdCustomer));
                propertyInfo.SetValue(normalizedCustomer, value);
                
            }
            return normalizedCustomer;
        }

        private float NormalizeDistibution(float customer, float medianCustomer, float stdCustomer)
        {
            return (float)Phi(customer, medianCustomer, stdCustomer);
        }

        private static double Phi(double z)
        {
            return 0.5 * (1.0 + erf(z / Math.Sqrt(2.0)));
        }
        private static double Phi(double z, double mu, double sigma)
        {
            return Phi((z - mu) / sigma);
        }

        private static double erf(double z)
        {
            var t = 1.0 / (1.0 + 0.5 * Math.Abs(z));

            // use Horner's method
            var ans = 1 -
                      t *
                      Math.Exp(
                          -z * z - 1.26551223 +
                          t *
                          (1.00002368 +
                           t *
                           (0.37409196 +
                            t *
                            (0.09678418 +
                             t *
                             (-0.18628806 +
                              t *
                              (0.27886807 +
                               t * (-1.13520398 + t * (1.48851587 + t * (-0.82215223 + t * 0.17087277)))))))));
            if (z >= 0)
            {
                return ans;
            }
            return -ans;
        }

            private float Normalize(float target, float min, float max)
        {
            if(target == 0)
            {
                return 0;
            }
            else
            {
                return (target - min) / (max - min);
            }

        }
    }
}
