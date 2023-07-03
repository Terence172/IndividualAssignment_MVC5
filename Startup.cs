using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IndividualAssignment_MVC5.Startup))]
namespace IndividualAssignment_MVC5
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
