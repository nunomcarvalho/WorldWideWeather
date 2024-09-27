namespace WorldWideWeather.Business.DTO
{
    public class UserDetails
    {
        public string? FirstName { set; get; }
        public string? LastName { set; get; }
        public string? Email { set; get; }
        public string? Address { set; get; }
        public DateTime? BirthDate { set; get; }
        public string? PhoneNumber { set; get; }
        public string? LivingCountry { set; get; }
        public string? CitizenCountry { set; get; }
    }

    public class UserInfo : UserDetails
    {
        public int Id { set; get; }
        public string Username { set; get; }
        public string Password { set; get; }
    }
}
