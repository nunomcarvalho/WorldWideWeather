using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WorldWideWeather.Business.Model;
using WorldWideWeather.Common.Country;
using WorldWideWeather.Controllers;
using WorldWideWeather.DTO;

namespace TestProject
{
    [TestClass]
    public class UnitTest
    {
        private IConfiguration _configuration;
        private ICountry _country;
        private SQLiteDbContext _dbContextMock;

        public UnitTest()
        {
            _configuration = ConfigurationSetup();
            _country = CountrySetup();
            _dbContextMock = DatabaseSetup();
        }

        private SQLiteDbContext DatabaseSetup()
        {
            SQLiteDbContext dbContext = new SQLiteDbContext(new Microsoft.EntityFrameworkCore.DbContextOptions<SQLiteDbContext>());
            User u = new User(dbContext);
            u.Registration(
                new WorldWideWeather.Business.DTO.UserDetails
                {
                    Address = "Rua de Cima",
                    BirthDate = DateTime.Now,
                    CitizenCountry = "MLT",
                    Email = "email1@company.com",
                    FirstName = "John",
                    LastName = "Doe",
                    LivingCountry = "MLT",
                    PhoneNumber = "+356 987987987",
                }, "xpto", _country
            );
            u.Registration(
                new WorldWideWeather.Business.DTO.UserDetails()
                {
                    Address = "Rua de Baixo",
                    BirthDate = DateTime.Now,
                    CitizenCountry = "SWE",
                    Email = "email2@company.com",
                    FirstName = "Jane",
                    LastName = "Doe",
                    LivingCountry = "SWE",
                    PhoneNumber = "+356 789789789"
                }, "xpto", _country
            );
            u.Registration(
                new WorldWideWeather.Business.DTO.UserDetails()
                {
                    Address = "Rua da Esquerda",
                    BirthDate = DateTime.Now,
                    CitizenCountry = "GBR",
                    Email = "email3@company.com",
                    FirstName = "Peter",
                    LastName = "Pan",
                    LivingCountry = "GBR",
                    PhoneNumber = "+356 987987987"
                }, "xpto", _country
            );
            u.Registration(
                new WorldWideWeather.Business.DTO.UserDetails()
                {
                    Address = "Rua da Direita",
                    BirthDate = DateTime.Now,
                    CitizenCountry = "MLT",
                    Email = "email4@company.com",
                    FirstName = "Joking",
                    LastName = "Clown",
                    LivingCountry = "MLT",
                    PhoneNumber = "+356 987987987"
                }, "xpto", _country
            );

            return dbContext;
        }

        private IConfiguration ConfigurationSetup()
        {
            //Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"ConnectionStrings:SQLite", "Data Source=SQLiteDb.db"},
                {"CountriesApi:Request", "https://restcountries.com/v3.1/alpha?codes="},
                {"Security:Secret", "d589cd29-1b1b-4055-b750-5c9bbe61139e"},
                {"Security:Issuer", "2811d0ea-4ea1-4670-b015-9c534a392461"},
                {"Security:Audience", "f6f2425d-9807-42cd-ba82-8c459958d400"},
                {"WeatherApi:Request", "https://api.open-meteo.com/v1/forecast?latitude={0}&longitude={1}&hourly=soil_temperature_0cm"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            return configuration;
        }

        private ICountry CountrySetup()
        {
            var url = _configuration.GetSection("CountriesApi").GetValue<string>("Request");
            var c = new Country(url);
            return c;
        }

        [TestMethod]
        public void RegisterUserSuccess()
        {
            RegistrationForm rf = new RegistrationForm();
            rf.Password = "xpto";
            rf.UserDetails = new UserDetails();
            rf.UserDetails.Address = "New amazing address";
            rf.UserDetails.BirthDate = DateTime.Now;
            rf.UserDetails.CitizenCountry = "MLT";
            rf.UserDetails.Email = "someemail@company.com";
            rf.UserDetails.FirstName = "Maria";
            rf.UserDetails.LastName = "Madalena";
            rf.UserDetails.LivingCountry = "MLT";
            rf.UserDetails.PhoneNumber = "+356 987987987";

            var c = new WeatherForecastController(null, _configuration, _country, _dbContextMock);

            var response = c.Registration(rf);

            Assert.IsInstanceOfType<OkResult>(response);
        }

        [TestMethod]
        public void LoginSuccess()
        {
            var c = new WeatherForecastController(null, _configuration, _country, _dbContextMock);
            var response = c.Login("jodo12345", "xpto");
            var result = (AuthenticateResponse)(((OkObjectResult)response).Value);

            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public void GetWeatherSuccess()
        {
            var c = new WeatherForecastController(null, _configuration, _country, _dbContextMock);
            var response = c.Weather("jodo12345");

            Assert.IsNotNull(response);
            Assert.IsNotNull(((OkObjectResult)response).Value);
        }

        [TestMethod]
        public void CountryGetPhonePrefixSuccess()
        {
            var phonePrefixMalta = _country.GetPhonePrefix("MLT");

            Assert.AreEqual(phonePrefixMalta, "+356", true);
        }

        [TestMethod]
        public void CountryGetLatLngSuccess()
        {
            var latLng = _country.GetLatLng("MLT");

            Assert.IsTrue(latLng != null);
            Assert.AreEqual(latLng.Item1, 35.9375M);
            Assert.AreEqual(latLng.Item2, 14.3754M);
        }
    }
}
