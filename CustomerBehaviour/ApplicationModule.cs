using Autofac;
using CustomerBehaviour.Application;
using CustomerBehaviour.Domain;
using CustomerBehaviour.Domain.LinearRegression;

namespace CustomerBehaviour
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
               .RegisterType<CustomerDataRetriever>()
               .AsSelf();

            builder
                .RegisterType<CustomerMappingEngine>()
                .AsSelf();

            builder
                .RegisterType<CustomerNormalizationEngine>()
                .AsSelf();

            builder
                .RegisterType<LinearRegression>()
                .AsSelf();
        }
    }
}