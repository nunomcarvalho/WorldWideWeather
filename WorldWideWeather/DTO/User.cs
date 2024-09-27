namespace WorldWideWeather.DTO
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

        // AutoMapper can be used instead
        public static explicit operator Business.DTO.UserDetails(UserDetails userDetails)
        {
            // if we want to throw an exception, this can be removed. (in this case, validation should be done before cast)
            if (userDetails == null) { return null; }

            var outer = new Business.DTO.UserDetails();
            outer.Address = userDetails.Address;
            outer.BirthDate = userDetails.BirthDate;
            outer.CitizenCountry = userDetails.CitizenCountry;
            outer.Email = userDetails.Email;
            outer.FirstName = userDetails.FirstName;
            outer.LastName = userDetails.LastName;
            outer.LivingCountry = userDetails.LivingCountry;
            outer.PhoneNumber = userDetails.PhoneNumber;

            return outer;
        }
    }

    public class UserInfo : UserDetails
    {
        public int Id { set; get; }
        public string Username { set; get; }
        public string Password { set; get; }
    }
}
