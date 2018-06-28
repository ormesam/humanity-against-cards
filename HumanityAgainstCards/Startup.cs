using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(HumanityAgainstCards.Startup))]
namespace HumanityAgainstCards
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}