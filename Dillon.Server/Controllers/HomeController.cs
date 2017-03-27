
namespace Dillon.Server.Controllers {
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web.Http;
    using WindowsInput;
    using WindowsInput.Native;

    public class InputCache
        : IInputCache {
        public int Cache { get; set; }
    }

    public interface IInputCache {
        int Cache { get; set; }
    }

    [RoutePrefix("")]
    public class HomeController
        : ApiController {

        public HomeController(IConfiguration config, IInputSimulator inputSimulator, IInputCache inputCache) {
            _config = config;
            _inputSimulator = inputSimulator;
            _inputCache = inputCache;
        }

        [HttpGet]
        [Route]
        public async Task<HttpResponseMessage> Index(string ui = "") {
            if (string.IsNullOrEmpty(ui))
                ui = _config.UI;
            //check configured ui exists. Maybe during startup and switch?
            var path = Path.Combine(_config.UIFolder, $"{ui}\\index.html");
            if (!File.Exists(path))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            try {
                response.Content = new StringContent(File.ReadAllText(path));
            } catch (Exception e) {
                throw new Exception($"There was a problem reading the HTML for the UI '{ui}'", e);
            }
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        [HttpGet]
        [Route]
        public async Task<IHttpActionResult> Index(int id, string value = "") {
            //var mapping = _config.Mappings[id];
            //var keyCode = ConvertToKeyCode(mapping);
            //_inputSimulator.Keyboard.KeyDown(VirtualKeyCode.F11);
            int amount = Convert.ToInt32(value);
            amount -= _inputCache.Cache;
            _inputCache.Cache += amount;
            //_inputSimulator.Mouse.MoveMouseBy(0, amount);
            _inputSimulator.Mouse.VerticalScroll(amount);
            return Ok();
        }

        private VirtualKeyCode ConvertToKeyCode(int value) {
            return (VirtualKeyCode)value;
        }

        private readonly IConfiguration _config;
        private readonly IInputSimulator _inputSimulator;
        private readonly IInputCache _inputCache;

    }
}