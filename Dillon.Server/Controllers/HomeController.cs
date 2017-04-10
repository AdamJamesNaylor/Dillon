
namespace Dillon.Server.Controllers {
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web.Http;
    using WindowsInput;
    using Common;
    using NLog;

    public class InputCache
        : IInputCache {
        public double Cache { get; set; }
    }

    public interface IInputCache {
        double Cache { get; set; }
    }

    [RoutePrefix("")]
    public class HomeController
        : ApiController {

        public HomeController(IConfiguration config, IInputCache inputCache) {
            _config = config;
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
        [Route("update")]
        public async Task<IHttpActionResult> Update([FromUri] Update[] updates) {

            foreach (var update in updates) {
                _log.Trace($"[UPDATE] {update.Id}: {update.Y}");

                if (_config.Mappings.ContainsKey(update.Id)) {
                    var mapping = _config.Mappings[update.Id];
                    mapping.Execute(update);
                    //_log.Trace($"Raised KeyPress for key code {keyCode}");
                } else {
                    double amount = Convert.ToDouble(update.Y);
                    amount -= _inputCache.Cache;
                    _inputCache.Cache += amount;
                    _inputSimulator.Mouse.VerticalScroll((int) amount);
                }
            }
            return Ok(updates.Length);
        }

        private readonly IConfiguration _config;
        private readonly IInputSimulator _inputSimulator;
        private readonly IInputCache _inputCache;
        private Logger _log = LogManager.GetCurrentClassLogger();
    }
}