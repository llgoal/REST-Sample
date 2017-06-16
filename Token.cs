using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.IO;

namespace DBI_REST_API
{
    class Token
    {
        static void Main(string[] args)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                // Get Session Id
                string host = "http://localhost:8001/";
                string logonUri = host + "Api/LogOn/Token";
                var logonOptions = new
                {
                    accountName = "admin",
                    password = "123456",
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

                string jsonString = string.Empty;

                using (var response = httpClient.PostAsync(logonUri, content).Result)
                {

                    //A <b>Dundas.BI.WebApi.Models.AccountData</b> object containing the account data as a JSON string.
                    string jsonObject = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(jsonObject);

                }

            }
            Console.Read();
        }

    }
}
