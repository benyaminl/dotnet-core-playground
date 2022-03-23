using System.Text;
using Microsoft.AspNetCore.Mvc;
using NATS.Client;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NatsController : ControllerBase {
        private readonly ILogger<NatsController> _log;
        private readonly IConnection _nats;

        public NatsController(ILogger<NatsController> log, 
            IConnection nats) {
            _log = log;
            _nats = nats;
        }

        [HttpGet("")]
        public async Task<ActionResult> tryGet() {
            var c = _nats;
            
            EventHandler<MsgHandlerEventArgs> h = async (sender, args) =>
            {
                // print the message
                Console.WriteLine(args.Message);

                // Here are some of the accessible properties from
                // the message:
                // args.Message.Data;
                // args.Message.Reply;
                // args.Message.Subject;
                // args.Message.ArrivalSubcription.Subject;
                // args.Message.ArrivalSubcription.QueuedMessageCount;
                // args.Message.ArrivalSubcription.Queue;
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("GET"), "http://localhost/coba.php"))
                    {
                        var response = await httpClient.SendAsync(request);
                        string content = await response.Content.ReadAsStringAsync();
                        _log.LogInformation(content);
                    }
                }
                // Unsubscribing from within the delegate function is supported.
                args.Message.ArrivalSubcription.Unsubscribe();
            };

            // The simple way to create an asynchronous subscriber
            // is to simply pass the event in.  Messages will start
            // arriving immediately.
            IAsyncSubscription s = c.SubscribeAsync("foo", h);

            // Alternatively, create an asynchronous subscriber on subject foo,
            // assign a message handler, then start the subscriber.   When
            // multicasting delegates, this allows all message handlers
            // to be setup before messages start arriving.
            // IAsyncSubscription sAsync = c.SubscribeAsync("foo");
            // sAsync.MessageHandler += h;
            // sAsync.Start();

            // Simple synchronous subscriber
            // ISyncSubscription sSync = c.SubscribeSync("foo");

            // Using a synchronous subscriber, gets the first message available,
            // waiting up to 1000 milliseconds (1 second)
            // Msg m = sSync.NextMessage(1000);

            c.Publish("foo", "whatever", Encoding.UTF8.GetBytes("hello world"));

            // Unsubscribing
            // sAsync.Unsubscribe();

            // Publish requests to the given reply subject:
            // c.Publish("foo", "bar", Encoding.UTF8.GetBytes("help!"));

            // Sends a request (internally creates an inbox) and Auto-Unsubscribe the
            // internal subscriber, which means that the subscriber is unsubscribed
            // when receiving the first response from potentially many repliers.
            // This call will wait for the reply for up to 1000 milliseconds (1 second).
            // Msg m = c.Request("foo", Encoding.UTF8.GetBytes("help"), 1000);
            return Ok();
        }

        [HttpGet("publish")]
        public ActionResult publish() {
            _nats.Publish("foo", "Doreply",Encoding.UTF8.GetBytes("Another test"));
            return Ok();
        }

        [HttpGet("request")]
        public ActionResult request() {
            // Still need to read this, why this is not working, and what is this used for?
            Msg m = _nats.Request("foo", Encoding.UTF8.GetBytes("Another test"));
            _nats.Close();
            return Ok(m);
        }
    }
}