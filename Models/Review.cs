using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jobseekr.Models
{
    [Table("ReviewsandRatings")]
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        public int JobId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string Id { get; set; }
    }
}