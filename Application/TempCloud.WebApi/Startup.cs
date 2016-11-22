﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using TempCloud.Service.Mappings;

[assembly: OwinStartup(typeof(TempCloud.WebApi.Startup))]

namespace TempCloud.WebApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);

            ConfigureOAuthTokenGeneration(app);
            ConfigureOAuthTokenConsumption(app);
            new Mappings();
        }
    }
}
