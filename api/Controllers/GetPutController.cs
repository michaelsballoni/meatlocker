using MeetLocker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Net.Http.Headers;
using System.Globalization;
using System.Text;

namespace api.Controllers
{
    [ApiController]
    [Route("api")]
    public class GetPutController : ControllerBase
    {
        public class PutForm
        {
            public string? pwd { get; set; }
            public string? filename { get; set; }
            public string? text { get; set; }
        }
        [HttpPost("put")]
        public async Task<ActionResult<string>> Put([FromForm] PutForm form)
        {
            string key;
            using (var text_stream = new MemoryStream(Encoding.UTF8.GetBytes(form.text ?? "")))
                key = await Mgr.Store(form.pwd ?? "", form.filename ?? "", text_stream);
            return Ok(key);
        }

        public class GetForm
        {
            public string? key { get; set; }
            public string? pwd { get; set; }
        }
        [HttpPost("get")]
        public async Task<ActionResult> Get([FromForm] GetForm form)
        {
            string output_filename = "";
            var output_stream = new MemoryStream();
            FileRetrieveHandler handler = Stream (string filename) => {
                output_filename = filename;
                return output_stream;
            };
            await Mgr.Retrieve(form.key ?? "", form.pwd ?? "", handler);
            Response.Headers.Append("X-Get-Filename", output_filename);
            await Response.BodyWriter.WriteAsync(output_stream.ToArray());
            return new EmptyResult();
        }
    }
}
