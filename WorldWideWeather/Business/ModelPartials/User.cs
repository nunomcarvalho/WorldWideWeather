using Microsoft.EntityFrameworkCore;
using WorldWideWeather.Business.DTO;
using WorldWideWeather.Common;
using WorldWideWeather.Common.Country;

namespace WorldWideWeather.Business.Model
{
    public partial class User
    {
        const string CacheKeyPrefix = "UserBusiness";

        private int _maxLength = 2;
        private static Random _rng = new Random();
        private SQLiteDbContext _dbContext;

        public User() : base() { }
        public User(SQLiteDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string Registration(UserDetails userDetails, string password, ICountry country)
        {
            AssertRegistration(userDetails, password, country);

            var user = new User();
            user.Address = userDetails.Address;
            user.BirthDate = userDetails.BirthDate;
            user.CitizenCountry = userDetails.CitizenCountry;
            user.LivingCountry = userDetails.LivingCountry;
            user.Email = userDetails.Email;
            user.FirstName = userDetails.FirstName;
            user.LastName = userDetails.LastName;
            user.Password = password; // shouldn't be in plain text
            user.PhoneNumber = userDetails.PhoneNumber;
            user.Username = GenerateUserName(userDetails.FirstName, userDetails.LastName);

            var latLng = country.GetLatLng(userDetails.LivingCountry);
            if (latLng != null)
            {
                user.Lat = latLng.Item1;
                user.Lng = latLng.Item2;
            }

            Add(user);

            return user.Username;
        }

        public bool Exists(string userName, string password)
        {
            var query = from u in _dbContext.Users
                        where u.Username.Equals(userName) && u.Password.Equals(password)
                        select u;

            return query.Any();
        }

        public bool Exists(string userName)
        {
            var query = from u in _dbContext.Users
                        where u.Username.Equals(userName)
                        select u;

            return query.Any();
        }

        public User Get(int id)
        {
            var key = $"{CacheKeyPrefix}GetUser{id}";
            var obj = CacheHelper.GetValue(key);
            if (obj != null) return (obj as User);

            var query = from u in _dbContext.Users
                        where u.Username.Equals(id)
                        select u;

            return query.SingleOrDefault();
        }
        public User Get(string userName)
        {
            var key = $"{CacheKeyPrefix}GetUser{userName}";
            var obj = CacheHelper.GetValue(key);
            if (obj != null) return (obj as User);

            var user = GetQuery(userName).SingleOrDefault(); // there should be only one

            if(user != null)
            {
                CacheHelper.AddValue(key, user);
            }
            return user;
        }
        public User Get(string userName, string password)
        {
            var key = $"{CacheKeyPrefix}GetUser{userName}";
            var obj = CacheHelper.GetValue(key);
            if (obj != null) return (obj as User);

            var query = from u in GetQuery(userName)
                        where u.Password.Equals(password)
                        select u;

            var user = query.SingleOrDefault(); // there should be only one

            if (user != null)
            {
                CacheHelper.AddValue(key, user);
            }
            return user;
        }

        private void Add(User user)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }

        private IQueryable<User> GetQuery(string userName)
        {
            var query = from u in _dbContext.Users
                        where u.Username.Equals(userName)
                        select u;
            return query.AsQueryable();
        }

        private string GenerateUserName(string firstName, string lastName)
        {
            var userNameBase = $"{firstName[.._maxLength]}{lastName[.._maxLength]}";
            var username = $"{userNameBase}{RandomInt()}".ToLower();

            while(Exists(username))
            {
                username = $"{userNameBase}{RandomInt()}".ToLower();
            }

            return username.ToLower();
        }

        private void AssertRegistration(UserDetails userDetails, string password, ICountry country)
        {
            // we can improve by creating a BusinessException
            if(string.IsNullOrWhiteSpace(password)) { throw new ArgumentNullException(nameof(password)); }
            if(string.IsNullOrWhiteSpace(userDetails.FirstName)) { throw new ArgumentNullException(nameof(userDetails.FirstName)); }
            if(string.IsNullOrWhiteSpace(userDetails.LastName)) { throw new ArgumentNullException(nameof(userDetails.LastName)); }
            if(string.IsNullOrWhiteSpace(userDetails.LivingCountry)) { throw new ArgumentNullException(nameof(userDetails.LivingCountry)); }

            if(string.IsNullOrWhiteSpace(userDetails.PhoneNumber)) { return; }

            var phonePrefix = country.GetPhonePrefix(userDetails.LivingCountry);
            if(string.IsNullOrWhiteSpace(phonePrefix)) { return; }

            if(!userDetails.PhoneNumber.StartsWith(phonePrefix))
            {
                throw new ArgumentException(nameof(userDetails.PhoneNumber));
            }
        }

        private static int RandomInt()
        {
            return _rng.Next(1, 99999);
        }
    }
}
