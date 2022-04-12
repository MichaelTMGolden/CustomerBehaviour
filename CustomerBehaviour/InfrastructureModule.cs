using Autofac;
using CustomerBehaviour.Application;
using CustomerBehaviour.Interfaces;

namespace CustomerBehaviour
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<DataReader>()
                .As<IDataReader>();

            builder
                .RegisterType<NormalizedDataWriter>()
                .As<INormalizedDataWriter>();
        }
    }
}