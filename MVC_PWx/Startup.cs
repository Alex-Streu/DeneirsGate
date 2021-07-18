using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(DeneirsGateSite.Startup))]
namespace DeneirsGateSite
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
