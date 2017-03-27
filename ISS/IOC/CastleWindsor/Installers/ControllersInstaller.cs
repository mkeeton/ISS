using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using System.Web.Mvc;

namespace ISS.IOC.CastleWindsor.Installers
{
    public class ControllersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {

            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));

            container.Register(Classes.FromThisAssembly()
                .BasedOn<Controller>()
                .LifestyleTransient()
            );

            container.Register(Classes.FromThisAssembly()
                .BasedOn<ApiController>()
                .LifestyleTransient()
            );
        }
    }
}