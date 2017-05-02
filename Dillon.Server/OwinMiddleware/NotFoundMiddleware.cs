
namespace Dillon.Server.OwinMiddleware {
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Owin;
    using Owin;

    class NotFoundMiddleware
        : OwinMiddleware {

        public NotFoundMiddleware(OwinMiddleware next, IAppBuilder app)
            : base(next) { }

        public override async Task Invoke(IOwinContext context) {
            await Next.Invoke(context);
            if (context.Response.StatusCode == 404) {
                using (StreamWriter writer = new StreamWriter(context.Response.Body)) {
                    string notFound = File.ReadAllText(@"Static\NotFound.html");
                    writer.Write(notFound);
                    writer.Flush();
                }
            }
        }
    }
}