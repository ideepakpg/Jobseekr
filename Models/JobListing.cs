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
        public string JobTitle { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public decimal Salary { get; set; }
        public DateTime PostedDate { get; set; }
    }
}