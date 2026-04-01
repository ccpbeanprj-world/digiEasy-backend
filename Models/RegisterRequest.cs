using System;

namespace digiEasy_backend.Models
{
    public class RegisterRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public string Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public string MobilePhoneNumber { get; set; }
    }
}
