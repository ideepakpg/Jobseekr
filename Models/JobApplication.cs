using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jobseekr.Models
{
    [Table("JobApplication")]
    public class JobApplication
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ApplicationId { get; set; }
        [ForeignKey("JobListing")]
        public int JobId { get; set; }
        [Required(ErrorMessage = "Applicant name is required")]
        public string ApplicantName { get; set; }
        [Required(ErrorMessage = "Applicant Email is required")]
        public string ApplicantEmail { get; set; }
        [Required(ErrorMessage = "Resume link is required")]
        public string Resume { get; set; }
        public JobListing JobListing { get; set; }
    }
}