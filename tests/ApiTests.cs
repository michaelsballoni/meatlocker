using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace tests
{
    [TestClass]
    internal class ApiTests
    {
        [TestMethod]
        public void TestGetPutGetGet()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost://5115");
            string filename = "test.txt";
            File.WriteAllText(filename, filename); // just for confusion
            byte[] file_bytes = File.ReadAllBytes(filename);

            //
            // PUT
            //
            var content = new ByteArrayContent(file_bytes);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");

            var request_content = new MultipartFormDataContent();
            request_content.Add(content, "text", "test.txt");

            var form_content = new FormUrlEncodedContent(new Dictionary<string, string>());
            request_content.Add(form_content);

            var response = client.PostAsync("localhost", request_content).Result;
            var response_string = response.Content.ReadAsStringAsync().Result;
        }


        // FORNOW

        //
        // GET - should work
        //

        //
        // GET - should not work
        //
    }
}
