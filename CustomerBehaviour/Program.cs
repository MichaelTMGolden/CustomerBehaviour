using System;
using System.Threading.Tasks;
using Autofac;
using CustomerBehaviour.Application;
using CustomerBehaviour.Domain.LinearRegression;

namespace CustomerBehaviour
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Creates Bootstraper
            using var container = Bootstrapper.Init();

            //Autofac build for Data Retiever
            var rawData = container.Resolve<CustomerDataRetriever>();

            var linear = container.Resolve<LinearRegression>();
            //Returns Normalized Customer Objects
            var customers = rawData.GetCustomersBehaviorSnapshots();

            linear.GetLinearRegression();
        }
    }
}
