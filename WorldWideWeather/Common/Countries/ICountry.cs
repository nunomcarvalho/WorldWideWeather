namespace WorldWideWeather.Common.Country
{
    public interface ICountry
    {
        public string GetPhonePrefix(string code);

        public Tuple<decimal, decimal> GetLatLng(string code);
    }
}
