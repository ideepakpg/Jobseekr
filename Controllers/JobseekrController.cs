using Jobseekr.Models;
using System.Web.Mvc;

namespace Jobseekr.Controllers
{
    public class JobseekrController : Controller
    {
        EmployeeDBContext obj = new EmployeeDBContext();
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
        public ActionResult Login()
        {
            // Add your login logic here
            return View();
        }

        // Action to handle user registration
        public ActionResult Register()
        {
            // Add your registration logic here
            return View();
        }

    }
}