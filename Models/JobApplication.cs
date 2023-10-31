using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jobseekr.Models
{
    [Table("JobApplication")]
    public class JobApplication
    {
        [Key]
        public int AppliedJobId { get; set; }
        public string ApplicantName { get; set; }
        public string JobTitle { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public decimal Salary { get; set; }
        public DateTime ApplicationDate { get; set; }
    }
}