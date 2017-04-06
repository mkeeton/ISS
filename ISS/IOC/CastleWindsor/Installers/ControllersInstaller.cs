using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using System.Web.Mvc;
using ISS.Authentication.Data.Interfaces.DbContext;
using ISS.Authentication.Data.Interfaces.Repositories;
using ISS.Authentication.Data.Repositories;
using ISS.Authentication.Data.DbContext;
using ISS.Authentication.Data.UnitOfWork;
//using ISS.Authentication.Infrastructure.Interfaces.Services;
using ISS.Authentication.Infrastructure.Factories;
//using ISS.Authentication.Infrastructure.Services.Email;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.DataHandler.Serializer;

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

            container.Register(
                Component.For<IDbContext>()
                    .UsingFactoryMethod(_ => new OleDapperDbContext(WebConfigurationManager.ConnectionStrings["AccessConnection"].ConnectionString)).LifestylePerWebRequest()
            );

            container.Register(
                      Component.For<IUserRepository>()
                      .ImplementedBy<UserRepository>()
                      .LifeStyle.PerWebRequest
            );

            container.Register(
                Component.For<ClientFactory>()
                .ImplementedBy<ClientFactory>()
                .LifeStyle.PerWebRequest
            );

            container.Register(
                Component.For<UserFactory>()
                .ImplementedBy<UserFactory>()
                .LifeStyle.PerWebRequest
            );

            //container.Register(
            //    Component.For<IEmailService>()
            //        .UsingFactoryMethod(_ => new SendGridEmailService(ConfigurationManager.AppSettings["emailService:APIKey"])).LifestylePerWebRequest()
            //);

            container.Register(
          Component.For<UnitOfWork>()
          .ImplementedBy<UnitOfWork>()
          .LifeStyle.PerWebRequest
      );

            container.Register(
                      Component.For<IClaimRepository>()
                      .ImplementedBy<ISS.Authentication.Data.Repositories.ClaimRepository>()
                      .LifeStyle.PerWebRequest
            );

            container.Register(
                      Component.For<IClientRepository>()
                      .ImplementedBy<ISS.Authentication.Data.Repositories.ClientRepository>()
                      .LifeStyle.PerWebRequest
            );

            container.Register(
                      Component.For<IRefreshTokenRepository>()
                      .ImplementedBy<ISS.Authentication.Data.Repositories.RefreshTokenRepository>()
                      .LifeStyle.PerWebRequest
            );

            container.Register(
                      Component.For<IPasswordResetTokenRepository>()
                      .ImplementedBy<ISS.Authentication.Data.Repositories.PasswordResetTokenRepository>()
                      .LifeStyle.PerWebRequest
            );

            //container.Register(
            //            Component.For<IEmailTemplateRepository>()
            //            .ImplementedBy<HighfieldABC.Authentication.Data.Repositories.EmailTemplateRepository>()
            //            .LifeStyle.PerWebRequest
            //);

            container.Register(
          Component.For<Managers.ApplicationUserManager>()
              .UsingFactoryMethod(_ => HttpContext.Current.GetOwinContext().GetUserManager<Managers.ApplicationUserManager>()).LifestylePerWebRequest()
      );

            container.Register(
                Component.For<ISecureDataFormat<AuthenticationTicket>>()
                    .UsingFactoryMethod(_ => new Providers.CustomJwtFormat("")).LifestylePerWebRequest()
            );

            container.Register(
                Component.For<ISecureDataFormat<AuthenticationTicket>>()
                .ImplementedBy<Providers.CustomJwtFormat>()
                .LifeStyle.PerWebRequest
            );


            container.Register(
                      Component.For<IDataSerializer<AuthenticationTicket>>()
                      .ImplementedBy<TicketSerializer>()
                      .LifeStyle.PerWebRequest
            );

            container.Register(
                Component.For<IDataProtector>()
                    .UsingFactoryMethod(_ => new DpapiDataProtectionProvider().Create("ASP.NET Identity")).LifestylePerWebRequest()
            );
        }
    }
}