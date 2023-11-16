using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jobseekr.Models
{
    [Table("Jobs")]
    public class JobListing
    {
        [Key]
        public int JobId { get; set; }
        [Required(ErrorMessage = "Job Title is required")]
        public string JobTitle { get; set; }
        [Required(ErrorMessage = "Job Description is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Job Location is required")]
        public string Location { get; set; }
        [Required(ErrorMessage = "Job Salary is required")]
        public decimal Salary { get; set; }
        [Required(ErrorMessage = "Job Posted Date is required")]
        public DateTime PostedDate { get; set; }
        // to store Id of employer who posted the job
        public int? EmployerId { get; set; }
    }
}