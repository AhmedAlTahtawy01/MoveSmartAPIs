using System.ComponentModel.DataAnnotations;

namespace Move_Smart.Models
{
    public class ChangePasswordModel
    {
        [Required]
        public required string OldPassword { get; set; }

        [Required]
        public required string NewPassword { get; set; }
    }
}