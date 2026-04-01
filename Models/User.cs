using System;
using System.ComponentModel.DataAnnotations;

namespace digiEasy_backend.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [MaxLength(20)]
        public string Gender { get; set; }

        [Required]
        public DateTime Birthday { get; set; }

        [Required]
        [MaxLength(8)]
        public string MobilePhoneNumber { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public DateTime RegistrationTimeUtc { get; set; }
    }
}
