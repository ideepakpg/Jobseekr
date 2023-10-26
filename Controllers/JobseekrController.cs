using Jobseekr.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Jobseekr.Controllers
{
    public class JobseekrController : Controller
    {
        private JobseekrDBContext obj = new JobseekrDBContext();
        // GET: Jobseekr
        public ActionResult Index()
        {
            return View();
        }

        // Action to handle landing page
        public ActionResult LandingPage()
        {
            // Add your logic here
            return View();
        }

        // Action to handle user login
        public ActionResult Login(EmployeeLogin model)
        {
            if (ModelState.IsValid)
            {
                var user = obj.employeeLogins.SingleOrDefault(u => u.Username == model.Username);

                if (user != null && user.Password == model.Password)
                {
                    FormsAuthentication.SetAuthCookie(model.Username, false);
                    return RedirectToAction("Welcome"); // Redirect to a secure page
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }
            return View(model);
        }

        // Action to handle user registration
        public ActionResult Register()
        {
            // Add your registration logic here
            return View();
        }

    }
}