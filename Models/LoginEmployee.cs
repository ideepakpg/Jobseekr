using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jobseekr.Models
{
    [Table("LoginEmployee")]
    public class LoginEmployee     //this model class is for only employees to login
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string Role { get; set; }
    }
}