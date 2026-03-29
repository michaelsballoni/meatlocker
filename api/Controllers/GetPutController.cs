using MeetLocker;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Web;

namespace api.Controllers
{
    [ApiController]
    [Route("api")]
    public class GetPutController : ControllerBase
    {
        [HttpPost("put")]
        public async Task<ActionResult<PutResponse>> Put(IFormFile file, [FromForm] string secret)
        {
            using (var file_stream = file.OpenReadStream()) {
                string key = await Mgr.Store(secret, file.FileName, file_stream);
                Response.ContentType = "application/json";
                return Ok(new PutResponse() { key = key });
            }
        }

        [HttpPost("get")]
        public async Task<ActionResult<string>> Get(GetClass get)
        {
            if (get.key == null || get.secret == null)
                Forbid();

            FileRetrieveHandler handler = Stream (string filename) => {
                var content_disposition = new ContentDispositionHeaderValue("attachment");
                content_disposition.SetHttpFileName(filename);
                Response.Headers[HeaderNames.ContentDisposition] = content_disposition.ToString();
                return Response.Body;
            };

            return Ok(Mgr.Retrieve(get.key, get.secret, handler));
        }
    }

    public class PutResponse
    {
        public string? key {  get; set; }
    }

    public class GetClass
    {
        public string? key { get; set; }
        public string? secret { get; set; }
    }
}
