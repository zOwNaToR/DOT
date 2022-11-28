using DataManager.Common.POCOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.DTOs
{
    public class RegisterUserRequest
    {
        [Required(ErrorMessage = "UserName required")]
        public string UserName { get; set; } = "";

        [Required(ErrorMessage = "First name required")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Last name required")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Birth date required")]
        public DateTime BirthDate { get; set; }

        [MaxLength(1, ErrorMessage = "Maximum {0} characters allowed")]
        public string? Sex { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email required")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password required")]
        public string Password { get; set; } = "";

        public User MapToPoco()
        {
            return new User
            {
                UserName = UserName,
                FirstName = FirstName,
                LastName = LastName,
                BirthDate = BirthDate,
                Sex = Sex,
                Email = Email,
                NormalizedEmail = Email.ToLower(),
                NormalizedUserName = UserName.ToLower(),
            };
        }
    }
}
