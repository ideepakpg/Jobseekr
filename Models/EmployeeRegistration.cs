using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jobseekr.Models
{
    [Table("EmployeeRegistration")]
    public class EmployeeRegistration
    {
        [Key]
        public int RegId { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Second Name is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email Id is required")]
        public string EmailId { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Mobile Number is required")]
        public string MobileNumber { get; set; }
    }
}