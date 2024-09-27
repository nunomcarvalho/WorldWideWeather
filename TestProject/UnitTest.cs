using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
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
        private Mock<SQLiteDbContext> _dbContextMock;
        private IList<User> users = new List<User>();

        public UnitTest()
        {
            _configuration = ConfigurationSetup();
            _country = CountrySetup();
            _dbContextMock = DatabaseSetup();
        }

        private Mock<SQLiteDbContext> DatabaseSetup()
        {
            users.Add(
                new User
                {
                    Address = "Rua de Cima",
                    BirthDate = DateTime.Now,
                    CitizenCountry = "MLT",
                    Email = "email1@company.com",
                    FirstName = "John",
                    LastName = "Doe",
                    LivingCountry = "MLT",
                    PhoneNumber = "+356 987987987",
                    Password = "xpto",
                    Username = "jodo12345",
                    Lat = 35.9375M,
                    Lng = 14.3754M
                }
            );
            users.Add(
                new User()
                {
                    Address = "Rua de Baixo",
                    BirthDate = DateTime.Now,
                    CitizenCountry = "SWE",
                    Email = "email2@company.com",
                    FirstName = "Jane",
                    LastName = "Doe",
                    LivingCountry = "SWE",
                    PhoneNumber = "+356 789789789",
                    Password = "xpto",
                    Username = "jodo12345",
                    Lat = 35.9375M,
                    Lng = 14.3754M
                }
            );
            
            var mockUser = new Mock<DbSet<User>>();
            mockUser.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.AsQueryable().Provider);
            mockUser.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.AsQueryable().Expression);
            mockUser.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.AsQueryable().ElementType);
            mockUser.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<SQLiteDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockUser.Object);

            return mockContext;
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

            var c = new WeatherForecastController(null, _configuration, _country, _dbContextMock.Object);

            var response = c.Registration(rf);

            Assert.IsInstanceOfType<OkResult>(response);
        }

        [TestMethod]
        public void LoginSuccess()
        {
            var c = new WeatherForecastController(null, _configuration, _country, _dbContextMock.Object);
            var response = c.Login("jodo12345", "xpto");
            var result = (AuthenticateResponse)(((OkObjectResult)response).Value);

            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public void GetWeatherSuccess()
        {
            var c = new WeatherForecastController(null, _configuration, _country, _dbContextMock.Object);
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
