
namespace Dillon.Client.Controllers {
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Text;
    using System.Web.Mvc;

    using System.Threading.Tasks;

    public struct Update {

        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class HomeController : Controller {
        public const int DefaultPort = 5000;
        public const string DefaultHostname = "192.168.1.145";

        public ActionResult Index() {
            return View();
        }

        //https://www.codeproject.com/kb/system/rawinput.aspx?fid=375378&df=90&mpp=25&noise=3&sort=position&view=quick&select=2811936
        public async Task SendUpdate(Update update) {
            var encodedUpdate = @"[{update.Key}:{update.Value}]";
            var bytesToSend = Encoding.ASCII.GetBytes(encodedUpdate);

            using (var client = new TcpClient(_hostname, _port)) {
                using (var stream = client.GetStream()) {

                    //---send the text---
                    Console.WriteLine("Sending - " + encodedUpdate);
                    stream.Write(bytesToSend, 0, bytesToSend.Length);

                    //---read back the text---
                    var bytesToRead = new byte[client.ReceiveBufferSize];
                    var bytesRead = stream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                    Console.WriteLine("Received - " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
                    Console.ReadLine();
                    stream.Close();
                }
                client.Close();
            }
        }

        private string _hostname = DefaultHostname;
        private int _port = DefaultPort;
    }
}