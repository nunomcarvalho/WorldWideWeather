using System.Globalization;
using System.Text.Json.Nodes;

namespace WorldWideWeather.Common.Country
{
    public class Country : ICountry
    {
        private readonly string _countryApiUrl;

        public Country(string countryApiUrl)
        {
            _countryApiUrl = countryApiUrl;
        }

        public string GetPhonePrefix(string code)
        {
            var result = ApiInvoker.Invoke($"{_countryApiUrl}{code}", code, RestSharp.Method.Get);

            if((result == null) || (result.StatusCode != System.Net.HttpStatusCode.OK) || (string.IsNullOrWhiteSpace(result.Content))) {  return string.Empty; }

            var jsonObject = JsonObject.Parse(result.Content);
            var idd = jsonObject[0]["idd"];
            var root = idd["root"].AsValue();
            var suffixes = idd["suffixes"];

            return $"{root}{suffixes[0]}";
        }

        public Tuple<decimal, decimal> GetLatLng(string code)
        {
            var result = ApiInvoker.Invoke($"{_countryApiUrl}{code}", code, RestSharp.Method.Get);

            if ((result == null) || (result.StatusCode != System.Net.HttpStatusCode.OK) || (string.IsNullOrWhiteSpace(result.Content))) { return null; }

            var jsonObject = JsonObject.Parse(result.Content);
            var latLng = jsonObject[0]["latlng"];

            decimal lat = decimal.Parse(latLng[0].ToString(), CultureInfo.InvariantCulture);
            decimal lng = decimal.Parse(latLng[1].ToString(), CultureInfo.InvariantCulture);

            return new Tuple<decimal, decimal>(lat, lng);
        }
    }
}
