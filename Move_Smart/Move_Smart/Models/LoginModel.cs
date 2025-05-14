using System.ComponentModel.DataAnnotations;

namespace Move_Smart.Models
{
    public class LoginModel
    {
        [Required]
        public required string NationalNo { get; set; }

        [Required, MinLength(6)]
        public required string Password { get; set; }
    }
}
