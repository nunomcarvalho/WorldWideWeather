using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using WorldWideWeather.Business.Model;
using WorldWideWeather.Common;
using WorldWideWeather.Common.Country;
using WorldWideWeather.Common.Security;
using WorldWideWeather.DTO;

namespace WorldWideWeather.Controllers
{
    [ApiController]
    [Route("[controller]/api")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _config;
        private readonly ICountry _country;
        private readonly SQLiteDbContext _dbContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, 
            IConfiguration config, ICountry country, SQLiteDbContext dbContext)
        {
            _logger = logger;
            _config = config;
            _country = country;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Returns the weather in a user's location.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("weather")]
        public IActionResult Weather(string username)
        {
            var userModel = new User(_dbContext);
            var user = userModel.Get(username);

            if (user == null) { return new UnauthorizedResult(); }

            var weatherUrl = string.Format(CultureInfo.InvariantCulture, 
                _config.GetSection("WeatherApi").GetValue<string>("Request"),
                user.Lat, user.Lng);

            var weather = new Weather(weatherUrl);

            var result = weather.Get(user.Lat, user.Lng);

            return Ok(result);
        }

        /// <summary>
        /// Creates a new user entry.
        /// </summary>
        /// <param name="registrationForm"></param>
        /// <returns></returns>
        [HttpPost("registration")]
        public IActionResult Registration(RegistrationForm registrationForm)
        {
            if (registrationForm == null || registrationForm.UserDetails == null)
            {
                return new BadRequestResult();
            }

            Business.DTO.UserDetails user = (Business.DTO.UserDetails)registrationForm.UserDetails;

            var userModel = new User(_dbContext);
            var username = userModel.Registration(user, registrationForm.Password, _country);

            return Ok(username);
        }

        /// <summary>
        /// Validates if user exists and returns a token if true.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public IActionResult Login(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password)) { return new BadRequestResult(); }

            var userModel = new User(_dbContext);
            var user = userModel.Get(userName, password);

            if (user == null) { return new UnauthorizedResult(); }

            var tokenHandler = new TokenManager(_config);

            var response = new AuthenticateResponse(user.Id, tokenHandler.GenerateToken(user));

            return Ok(response);
        }
    }
}
