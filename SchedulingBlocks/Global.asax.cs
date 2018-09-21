using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Extensions;
using SimpleInjector.Integration.Web.Mvc;
using SchedulingBlocks.Interfaces;
using SchedulingBlocks.Repositories;

namespace SchedulingBlocks
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static Container container;

        public static Container Container
        {
            get
            {
                return container;
            }
        }

        protected void Application_Start()
        {
            /////////////////////////////////Dependency Injection ////////////////////////////////
            container = new Container();
            InitializeContainer();

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.RegisterMvcIntegratedFilterProvider();
            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }

        private void InitializeContainer()
        {
            //Repositories
            container.RegisterPerWebRequest<IAppDbRepository, MockRepository>();

            //ORM Entities
        }
    }
}