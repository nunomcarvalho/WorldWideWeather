using RestSharp;
using System.Net;

namespace WorldWideWeather.Common
{
    public class ApiInvoker
    {
        public static RestResponse Invoke(string endPoint, string body, Method verbMethod)
        {
            if (endPoint == null) { return null; }

            var client = new RestClient(endPoint);

            var request = new RestRequest("", verbMethod);
            request.AddHeader("accept", "application/json");
            if (verbMethod != Method.Get) { request.AddJsonBody(body); }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var response = client.Execute(request);
            return response;
        }
    }
}
