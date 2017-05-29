using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.IO;
 
namespace DBI_REST_API
{
    class Class1
    {
        static void Main(string[] args)
        {

            using (HttpClient client = new HttpClient())
            {
                string host = "http://localhost:8001/";
                //change the logon credentials to match your own DBI instance and account
                string sessionID = GetSessionId(client, host, "admin", "123456");
                //Console.WriteLine(sessionID);
                //change providerId to the one that corresponds with your Export Provider, the following ID is for exporting to PDF
                string providerId = "5752eb39-40b5-4d79-b96b-9f9297c67193";
                //replace the guid view the guid the corresponds to the Dashboard you would like to export
                string viewID = "c82f0217-2d5d-4137-ba23-28f8c71a0830";
                string viewUrl = host + "Dashboard/" + viewID + "?e=false&vo=none";
                string exportQueryUri = host + "api/Export/?sessionId=" + sessionID + "";

                // Define the request body
                string exportOptions = "{ \"viewUrl\": \"" + viewUrl + "\"," + "\"isLegacyExport\": true, \"viewId\": \"" + viewID + "\", \"providerId\": \"" + providerId + "\", \"parameterValues\": [], \"requests\": []}";
                HttpContent requestBody = null;
                requestBody = new StringContent(exportOptions, Encoding.UTF8, "application/json");

                using (HttpResponseMessage response = client.PostAsync(exportQueryUri, requestBody).Result)
                {
                    string jsonString = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        
                        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(string.Format(host + "export/?exportId={0}", jsonString.Replace("\"", string.Empty)));
                        webRequest.CookieContainer = new CookieContainer();
                        Cookie cookie = new Cookie("dundas_webapp_sessionid", sessionID, "/");
                        webRequest.CookieContainer.Add(new Uri(host), cookie); 
                        HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                        Stream responseStream = webResponse.GetResponseStream();
                        StreamReader streamReader = new StreamReader(responseStream);
                        string s = streamReader.ReadToEnd();
                        //string 's' now contains the export and can be written into a file
                        Console.WriteLine(s);
                    }
                }
            }
            Console.Read();

        }

        private static string GetSessionId(HttpClient client, string baseUri, string username, string password)
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

                    var obj = (Dictionary<string, object>)serializer.DeserializeObject(jsonString);

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

