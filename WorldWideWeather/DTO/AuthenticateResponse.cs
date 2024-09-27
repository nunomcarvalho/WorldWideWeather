using WorldWideWeather.Business.Model;

namespace WorldWideWeather.DTO
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string Token { get; set; }

        public AuthenticateResponse(int userId, string token)
        {
            Id = userId;
            Token = token;
        }
    }
}
