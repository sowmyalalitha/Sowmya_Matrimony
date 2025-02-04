using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp.netwebmvcWebsite.Models
{
    public class User
    {
        [Key] // Marks this property as the primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Sets auto-increment
        public int SNO { get; set; }

        [Required(ErrorMessage = "Email is Requried")]
        [RegularExpression(@"[a-zA-Z0-9.-]+(.[a-zA-Z]{2,})+", ErrorMessage = "Enter a valid Email.")]
        public string RegisteredEmailId { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,15}$",
        ErrorMessage = "Password should contain UpperCase, Lowercase, Special Character, Numeric Digit")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,15}$",
        ErrorMessage = " Conform Password should contain UpperCase, Lowercase, Special Character, Numeric Digit")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConformPassword { get; set; }
        public DateTimeOffset RegisteredDateTime { get; set; }

    }
}

