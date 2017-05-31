using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using System;
using System.Text;

namespace DBI_REST_API
{
    class Program
    {
        static void Main(string[] args)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string sessionId = GetSessionId(
                        httpClient,
                        "http://localhost:8000/",
                        "admin",
                        "123456"
                        );
                Console.WriteLine(sessionId);
            }
            Console.Read();
        }


        private static string GetSessionId(
            HttpClient client,
            string baseUri,
            string username,
            string password
        )
        { 

            string logonUri = baseUri + "Api/LogOn/";
            string result = string.Empty;

            var logonOptions = new
            {
                accountName = username,
                password = password,
                cultureName = string.Empty,
                deleteOtherSessions = false,
                isWindowsLogOn = false

            };

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var requestBodyAsString = serializer.Serialize(logonOptions);

            StringContent content =
                new StringContent(
                    requestBodyAsString,
                    Encoding.UTF8,
                    "application/json"
                    );

            using (var response = client.PostAsync(logonUri, content).Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    string jsonString =
                        response.Content.ReadAsStringAsync().Result;

                    var obj = (System.Collections.Generic.Dictionary<string, object>)serializer.DeserializeObject(jsonString);

                    if (obj["logOnFailureReason"].ToString() == "None")
                    {
                        result = obj["sessionId"].ToString();
                    }
                    else
                    {
                        throw new Exception("Login failed");
                    }

                }

            }

            return result;
        }
    }
}
