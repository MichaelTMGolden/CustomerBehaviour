using Autofac;

namespace CustomerBehaviour
{
    internal class Bootstrapper
    {
        public static IContainer Init()
        {
            var builder = new ContainerBuilder();

            builder
               .RegisterModule<InfrastructureModule>();

            builder
               .RegisterModule<ApplicationModule>();

            return builder.Build();
        }
    }
}