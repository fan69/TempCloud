﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.DataHandler.Serializer;
using Microsoft.Owin.Security.DataProtection;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TempCloud.DataModel.Models;
using TempCloud.Service.Interfaces;
using TempCloud.Service.Services;
using static TempCloud.WebApi.ApplicationUserManager;

namespace TempCloud.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebApiRequestLifestyle();


            //Authentication 
            container.Options.AllowOverridingRegistrations = true;

            container.Register<IUserStore<ApplicationUser>>(
                () => new UserStore<ApplicationUser>(new ApplicationDbContext()), Lifestyle.Scoped);
            container.Register<ApplicationUserManager>(Lifestyle.Scoped);

            container.Options.AllowOverridingRegistrations = false;
            container.Register<ISecureDataFormat<AuthenticationTicket>, SecureDataFormat<AuthenticationTicket>>(Lifestyle.Scoped);
            container.Register<ITextEncoder, Base64UrlTextEncoder>(Lifestyle.Scoped);
            container.Register<IDataSerializer<AuthenticationTicket>, TicketSerializer>(Lifestyle.Scoped);
            container.Register<IDataProtector>(() => new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider().Create("ASP.NET Identity"), Lifestyle.Scoped);


            //Controllers
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);


            //Services
            container.Register<IDeviceService, DeviceService>(Lifestyle.Transient);
            container.Register<IUserService, UserService>(Lifestyle.Transient);

            container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);
        }

        void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type,Content-Range, Content-Disposition, Content-Description, Authorization, X-Requested-With");
            HttpContext.Current.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE,OPTIONS, X-Requested-With");
            HttpContext.Current.Response.Headers.Remove("Allow");
            HttpContext.Current.Response.Headers.Add("Allow", "GET, POST, PUT, DELETE, OPTIONS, X-Requested-With");
            HttpContext.Current.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
               // HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");

               // HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept");
                //HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Authorization");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
                HttpContext.Current.Response.StatusCode = 200;
                HttpContext.Current.Response.End();
            }
        }
    }
}
