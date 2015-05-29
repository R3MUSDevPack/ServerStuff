using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(r3mus.Startup))]
namespace r3mus
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
