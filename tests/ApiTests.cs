using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace tests
{
    [TestClass]
    public class ApiTests
    {
        private Uri ToUrl(string baseUri, string stemUri)
        {
            return new Uri(baseUri + stemUri);
        }

        [TestMethod]
        public void TestGetPutGetGet()
        {
            HttpClient client = new HttpClient();
            string base_uri = "http://localhost:5115/api/";
            string filename = "test.txt";
            File.WriteAllText(filename, filename); // just for confusion
            byte[] file_bytes = File.ReadAllBytes(filename);
            string key = "";

            //
            // PUT
            //
            {
                var byte_content = new ByteArrayContent(file_bytes);
                byte_content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");

                var request_content = new MultipartFormDataContent();
                request_content.Add(byte_content, "file", filename);

                var form_dict = new Dictionary<string, string>();
                form_dict["secret"] = "shhh";
                var form_content = new FormUrlEncodedContent(form_dict);
                request_content.Add(form_content);

                var response = client.PostAsync(ToUrl(base_uri, "put"), request_content).Result;
                Assert.IsTrue(response.IsSuccessStatusCode);
                key = response.Content.ReadAsStringAsync().Result;
                File.WriteAllText(filename + ".key", key);
            }

            //
            // GET
            //
            {
                var form_dict = new Dictionary<string, string>();
                form_dict["key"] = key;
                form_dict["secret"] = "shhh";
                var form_content = new FormUrlEncodedContent(form_dict);
                var response = client.PostAsync(ToUrl(base_uri, "get"), form_content).Result;
                string response_txt = response.Content.ReadAsStringAsync().Result;
                Assert.AreEqual(filename, response_txt);
            }
        }

        // FORNOW

        //
        // GET - should not work
        //
    }
}
