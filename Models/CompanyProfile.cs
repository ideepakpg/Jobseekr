using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jobseekr.Models
{
    [Table("CompanyProfile")]
    public class CompanyProfile
    {
        [Key]
        public int CompanyId { get; set; }
        [Required(ErrorMessage = "Company Name is required")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Company Description is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Job Location is required")]
        public string Location { get; set; }
        [Required(ErrorMessage = "Year is required")]
        public int Founded { get; set; }
        [Required(ErrorMessage = "Contact number is required")]
        public long Contact { get; set; }
        [Required(ErrorMessage = "Website is required")]
        public string Website { get; set; }
    }
}