﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MVC_PWx.Startup))]
namespace MVC_PWx
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
