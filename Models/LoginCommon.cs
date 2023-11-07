using System.ComponentModel.DataAnnotations;

namespace Jobseekr.Models
{
    public class LoginCommon    // to login for both employee and employer (for Login view model to change model from Jobseekr.Models.Login to Jobseekr.Models.LoginCommon)
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