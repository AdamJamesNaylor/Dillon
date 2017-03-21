
namespace Dillon.Server.Controllers {
    using System.Threading.Tasks;
    using System.Web.Http;
    using WindowsInput;
    using WindowsInput.Native;

    [RoutePrefix("")]
    public class HomeController
        : ApiController {

        public HomeController() {
            
        }

        [HttpGet]
        [Route]
        public async Task<IHttpActionResult> Index(int key, string value) {
            var i = new InputSimulator();
            i.Keyboard.KeyDown(VirtualKeyCode.F11);
            
            return Ok();
        }
    }
}