using MeetLocker;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<string?>> Put([FromForm] PutForm form)
        {
            string? key;
            using (var text_stream = new MemoryStream(Encoding.UTF8.GetBytes(form.text ?? "")))
                key = await Mgr.Store(form.pwd ?? "", form.filename ?? "filename", text_stream);
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
            FileRetrieveHandler handler = Stream (string filename) => {
                return Response.Body;
            };
            if (!await Mgr.Retrieve(form.key ?? "", form.pwd ?? "", handler))
                return BadRequest();
            return new EmptyResult();
        }
    }
}
