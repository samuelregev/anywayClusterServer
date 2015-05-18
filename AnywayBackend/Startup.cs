using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AnywayBackend.Startup))]
namespace AnywayBackend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
