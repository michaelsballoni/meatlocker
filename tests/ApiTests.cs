using System.Net.Http.Headers;

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

            string test_string = "test string";

            string filename = "test.txt";
            File.WriteAllText(filename, test_string);

            byte[] file_bytes = File.ReadAllBytes(filename);

            string key = "";
            string pwd = "shhh";

            //
            // PUT
            //
            {
                var byte_content = new ByteArrayContent(file_bytes);
                byte_content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");

                var request_content = new MultipartFormDataContent();
                request_content.Add(byte_content, "file", filename);

                var form_dict = new Dictionary<string, string>();
                form_dict["pwd"] = pwd;
                var form_content = new FormUrlEncodedContent(form_dict);
                request_content.Add(form_content);

                var put_response = client.PostAsync(ToUrl(base_uri, "put"), request_content).Result;
                Assert.IsTrue(put_response.IsSuccessStatusCode);
                key = put_response.Content.ReadAsStringAsync().Result;
            }

            //
            // GET
            //
            {
                var form_dict = new Dictionary<string, string>();
                form_dict["key"] = key;
                form_dict["pwd"] = pwd;
                var form_content = new FormUrlEncodedContent(form_dict);
                var response = client.PostAsync(ToUrl(base_uri, "get"), form_content).Result;
                string response_txt = response.Content.ReadAsStringAsync().Result;
                Assert.AreEqual(test_string, response_txt);
            }

            //
            // GET - should not work
            //
            {
                var form_dict = new Dictionary<string, string>();
                form_dict["key"] = key;
                form_dict["pwd"] = pwd;
                var form_content = new FormUrlEncodedContent(form_dict);
                var response = client.PostAsync(ToUrl(base_uri, "get"), form_content).Result;
                try
                {
                    string response_txt = response.Content.ReadAsStringAsync().Result;
                    Assert.AreEqual(test_string, response_txt);
                    Assert.Fail();
                }
                catch { }
            }
        }
    }
}
