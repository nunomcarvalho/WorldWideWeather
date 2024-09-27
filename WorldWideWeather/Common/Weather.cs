using System.Text.Json;

namespace WorldWideWeather.Common
{
    public class Weather
    {
        private readonly string _weatherUrl;

        public Weather(string url)
        {
            _weatherUrl = url;
        }

        public string Get(decimal lat, decimal lng)
        {
            var location = new Location();
            location.Lat = lat;
            location.Lng = lng;

            var body = JsonSerializer.Serialize(location);

            var result = ApiInvoker.Invoke(_weatherUrl, body, RestSharp.Method.Get);
            if ((result == null) || (result.StatusCode != System.Net.HttpStatusCode.OK) || (string.IsNullOrWhiteSpace(result.Content))) { return string.Empty; }

            return result.Content;
        }

        class Location
        {
            public decimal Lat { set; get; }
            public decimal Lng { set; get; }
        }
    }
}
