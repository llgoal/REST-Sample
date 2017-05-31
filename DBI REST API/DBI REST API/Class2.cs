using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.IO;

namespace DBI_REST_API
{
    class Class2
    {
        static void Main(string[] args)
        { 
            using (HttpClient httpClient = new HttpClient())
            {
                // Get Session Id
                string logonUri = "http://localhost:8000/Api/LogOn/";
                var logonOptions = new
                {
                    accountName = "admin",
                    password = "123456",
                    cultureName = string.Empty,
                    deleteOtherSessions = true,
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
                    jsonString =
                        response.Content.ReadAsStringAsync().Result;
                }
                
                var obj = (Dictionary<string, object>)serializer.DeserializeObject(jsonString);
                string sessionId = obj["sessionId"].ToString();
                string url = "http://localhost:8000/api/Account/588f1498-6fc8-4cdf-8b99-a514da16b0e7/?sessionId=" + sessionId + "";
                
                using (var response = httpClient.GetAsync(url).Result)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Success");
                        
                        //A <b>Dundas.BI.WebApi.Models.AccountData</b> object containing the account data as a JSON string.
                        string jsonObject = response.Content.ReadAsStringAsync().Result;
                        Console.WriteLine(jsonObject);
                    } else
                    {
                        Console.WriteLine(response);
                    }

                }
            }
            Console.Read();
        }
        
    }
}
