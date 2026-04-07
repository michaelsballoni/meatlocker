using MeetLocker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace api.Controllers
{
    [ApiController]
    [Route("api")]
    public class GetPutController : ControllerBase
    {
        [HttpPost("put")]
        public async Task<ActionResult<string>> Put([FromForm] string pwd, [FromForm] IFormFile file)
        {
            pwd = TrimParam(pwd);
            string key;
            using (var file_stream = file.OpenReadStream())
                key = await Mgr.Store(pwd, file.FileName, file_stream);
            return Ok(key);
        }

        [HttpPost("get")]
        public async Task<ActionResult> Get([FromForm] string key, [FromForm] string pwd)
        {
            key = TrimParam(key);
            pwd = TrimParam(pwd);
            string output_filename = "";
            FileRetrieveHandler handler = Stream (string filename) => {
                output_filename = filename;
                var content_disposition = new ContentDispositionHeaderValue("attachment");
                content_disposition.SetHttpFileName(output_filename);
                Response.Headers[HeaderNames.ContentDisposition] = content_disposition.ToString();
                return Response.Body;
            };

            await Mgr.Retrieve(key, pwd, handler);
            return new EmptyResult();
        }

        private static string TrimParam(string param)
        {
            int eq = param.IndexOf('=');
            return eq > 0 ? param.Substring(eq + 1) : param;
        }
    }
}
