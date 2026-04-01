using System;
using System.Linq;
using System.Text.RegularExpressions;
using digiEasy_backend.Data;
using digiEasy_backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace digiEasy_backend.Controllers
{
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly PasswordHasher<User> _passwordHasher;

        public UsersController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _passwordHasher = new PasswordHasher<User>();
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { error = "Request body is required." });
            }

            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.PasswordConfirmation) ||
                string.IsNullOrWhiteSpace(request.Gender) ||
                !request.Birthday.HasValue ||
                string.IsNullOrWhiteSpace(request.MobilePhoneNumber))
            {
                return BadRequest(new { error = "All fields are required." });
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            if (!Regex.IsMatch(normalizedEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return BadRequest(new { error = "Invalid email format." });
            }

            if (_dbContext.Users.Any(u => u.Email == normalizedEmail))
            {
                return BadRequest(new { error = "Email has already been registered." });
            }

            if (!IsAtLeast18(request.Birthday.Value))
            {
                return BadRequest(new { error = "User must be at least 18 years old." });
            }

            if (!Regex.IsMatch(request.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$"))
            {
                return BadRequest(new { error = "Password must be at least 8 characters and include uppercase, lowercase, and a digit." });
            }

            if (request.Password != request.PasswordConfirmation)
            {
                return BadRequest(new { error = "Password confirmation must match password." });
            }

            var normalizedGender = request.Gender.Trim().ToUpperInvariant();
            if (normalizedGender != "M" && normalizedGender != "F")
            {
                return BadRequest(new { error = "Gender must be M or F." });
            }

            if (!Regex.IsMatch(request.MobilePhoneNumber, @"^\d{8}$"))
            {
                return BadRequest(new { error = "Mobile phone number must be exactly 8 digits." });
            }

            var user = new User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = normalizedEmail,
                Gender = normalizedGender,
                Birthday = request.Birthday.Value.Date,
                MobilePhoneNumber = request.MobilePhoneNumber.Trim(),
                RegistrationTimeUtc = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return Ok(new
            {
                message = "Registration successful.",
                registrationTimeUtc = user.RegistrationTimeUtc
            });
        }

        [HttpPatch("password")]
        public IActionResult UpdatePassword([FromBody] UpdatePasswordRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { error = "Request body is required." });
            }

            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.CurrentPassword) ||
                string.IsNullOrWhiteSpace(request.NewPassword) ||
                string.IsNullOrWhiteSpace(request.NewPasswordConfirmation))
            {
                return BadRequest(new { error = "All fields are required." });
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = _dbContext.Users.SingleOrDefault(u => u.Email == normalizedEmail);
            if (user == null)
            {
                return BadRequest(new { error = "User does not exist." });
            }

            var passwordCheck = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
            if (passwordCheck == PasswordVerificationResult.Failed)
            {
                return BadRequest(new { error = "Current password is incorrect." });
            }

            if (!Regex.IsMatch(request.NewPassword, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$"))
            {
                return BadRequest(new { error = "New password must be at least 8 characters and include uppercase, lowercase, and a digit." });
            }

            if (request.CurrentPassword == request.NewPassword)
            {
                return BadRequest(new { error = "New password must be different from current password." });
            }

            if (request.NewPassword != request.NewPasswordConfirmation)
            {
                return BadRequest(new { error = "New password confirmation must match new password." });
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
            _dbContext.SaveChanges();

            return Ok(new { message = "Password updated successfully." });
        }

        private static bool IsAtLeast18(DateTime birthday)
        {
            var today = DateTime.UtcNow.Date;
            var age = today.Year - birthday.Year;
            if (birthday.Date > today.AddYears(-age))
            {
                age--;
            }

            return age >= 18;
        }
    }
}
