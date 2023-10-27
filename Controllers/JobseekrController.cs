using Jobseekr.Models;
using System.Linq;
using System.Web.Mvc;

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
        public ActionResult LandingPage(string viewName = "LandingPage")
        {

            if (viewName == "Login")
            {
                // to redirect to Login page
                return View("Login");
            }
            else if (viewName == "Registration")
            {
                // to redirect to Registration page
                return View("Registration");
            }
            return View("LandingPage");
        }

        // Action to handle user login
        public ActionResult Login(EmployeeLogin model)
        {
            if (ModelState.IsValid)
            {
                using (var dbContext = new JobseekrDBContext())
                {
                    var user = obj.employeeLogins.SingleOrDefault(u => u.Username == model.Username);

                    if (user != null && user.Password == model.Password)
                    {

                        //FormsAuthentication.SetAuthCookie(model.Username, false);

                        return RedirectToAction("WelcomePage");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password.");
                    }
                }
            }
            return View(model);
        }

        public ActionResult WelcomePage()
        {
            var jobListings = obj.jobListings.ToList();
            return View(jobListings);
        }


        // Action to handle user registration
        [HttpGet]
        public ActionResult Registration()
        {

            EmployeeRegistration obj2 = new EmployeeRegistration();
            obj.employeeRegistrations.Add(obj2);
            // If registration data is not valid, redisplay the registration form with validation errors
            return View();
        }

        [HttpPost]
        public ActionResult Registration(EmployeeRegistration employeeRegistrations)
        {
            if (ModelState.IsValid)
            {
                using (var dbContext = new JobseekrDBContext())
                {
                    // Create instances for both models (to insert in both tables)
                    var registration = new EmployeeRegistration
                    {
                        FirstName = employeeRegistrations.FirstName,
                        LastName = employeeRegistrations.LastName,
                        EmailId = employeeRegistrations.EmailId,
                        Username = employeeRegistrations.Username,
                        Password = employeeRegistrations.Password,
                        MobileNumber = employeeRegistrations.MobileNumber
                    };

                    var login = new EmployeeLogin
                    {
                        Username = employeeRegistrations.Username,
                        Password = employeeRegistrations.Password
                    };

                    // Add and save the instances to their respective tables
                    dbContext.employeeRegistrations.Add(registration);
                    dbContext.employeeLogins.Add(login);
                    dbContext.SaveChanges();
                }

                return RedirectToAction("RegistrationSuccess");
            }

            return View();
        }


        public ActionResult RegistrationSuccess()
        {
            // logic here
            return View();
        }
    }
}