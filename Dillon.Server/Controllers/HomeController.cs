
namespace Dillon.Server.Controllers {
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web.Http;
    using WindowsInput;
    using WindowsInput.Native;

    [RoutePrefix("")]
    public class HomeController
        : ApiController {

        public HomeController(IConfiguration config, IInputSimulator inputSimulator) {
            _config = config;
            _inputSimulator = inputSimulator;
        }

        [HttpGet]
        [Route]
        public async Task<HttpResponseMessage> Index() {
            //check configured ui exists. Maybe during startup and switch?
            var path = Path.Combine(_config.UIFolder, $"{_config.UI}\\index.html");
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(File.ReadAllText(path));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        [HttpGet]
        [Route]
        public async Task<IHttpActionResult> Index(int id, string value = "") {
            var mapping = _config.Mappings[id];
            var keyCode = ConvertToKeyCode(mapping);
            _inputSimulator.Keyboard.KeyDown(keyCode);

            return Ok();
        }

        private VirtualKeyCode ConvertToKeyCode(int value) {
            return (VirtualKeyCode)value;
        }

        private readonly IConfiguration _config;
        private readonly IInputSimulator _inputSimulator;
    }
}