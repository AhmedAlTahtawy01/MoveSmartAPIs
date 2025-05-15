using System.ComponentModel.DataAnnotations;

namespace Move_Smart.Models
{
    public class DataRangeModel
    {
        [Required]
        public required DateTime StartDate { get; set; }

        [Required]
        public required DateTime EndDate { get; set; }
    }
}
