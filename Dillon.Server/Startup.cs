
namespace Dillon.Server {
    using System.Web.Http;
    using Owin;

    public class Startup {

        public void Configuration(IAppBuilder appBuilder) {

            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            appBuilder.UseWebApi(config);
        }
    }
}