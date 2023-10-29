using Jobseekr.Models;
using System.Data.Entity;
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
                    var user = obj.employeeLogins.FirstOrDefault(u => u.Username == model.Username);

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

        // job provider aka employer section starts here

        public ActionResult WelcomePage()
        {
            var jobListings = obj.jobListings.ToList();
            return View(jobListings);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(JobListing jobListings)
        {
            if (ModelState.IsValid)
            {
                using (var dbContext = new JobseekrDBContext())
                {
                    // Add the job listing to the database
                    dbContext.jobListings.Add(jobListings);
                    dbContext.SaveChanges();
                }

                return RedirectToAction("WelcomePage"); // Redirect to the WelcomePage action
            }

            return View(jobListings);
        }

        public ActionResult Edit(int id)
        {
            using (var dbContext = new JobseekrDBContext())
            {
                // Retrieve the job listing by ID
                var jobListing = dbContext.jobListings.Find(id);

                if (jobListing == null)
                {
                    return HttpNotFound(); // or return an appropriate error view
                }

                return View(jobListing);
            }
        }

        [HttpPost]
        public ActionResult Edit(JobListing jobListing)
        {
            if (ModelState.IsValid)
            {
                using (var dbContext = new JobseekrDBContext())
                {
                    dbContext.Entry(jobListing).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }

                return RedirectToAction("WelcomePage");
            }

            return View(jobListing);
        }

        public ActionResult Details(int id)
        {
            using (var dbContext = new JobseekrDBContext())
            {
                // Retrieve the job listing by ID
                var jobListing = dbContext.jobListings.Find(id);

                if (jobListing == null)
                {
                    return HttpNotFound(); // or return an appropriate error view
                }

                return View(jobListing);
            }
        }

        public ActionResult Delete(int id)
        {
            using (var dbContext = new JobseekrDBContext())
            {
                // Retrieve the job listing by ID
                var jobListing = dbContext.jobListings.Find(id);

                if (jobListing == null)
                {
                    return HttpNotFound(); // error view
                }

                return View(jobListing);
            }
        }

        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var dbContext = new JobseekrDBContext())
            {
                // Retrieve the job listing by ID
                var jobListing = dbContext.jobListings.Find(id);

                if (jobListing != null)
                {
                    dbContext.jobListings.Remove(jobListing);
                    dbContext.SaveChanges();
                }

                return RedirectToAction("WelcomePage");
            }
        }

        // job provider aka employer section ends here


        // job employee aka seeker section starts here

        public ActionResult AvailableJobs()
        {
            var jobListings = obj.jobListings.ToList();
            return View(jobListings);
        }

        public ActionResult ApplyForJob(int id)
        {
            // Retrieve the job listing by its ID
            var jobListing = obj.jobListings.Find(id);

            if (jobListing == null)
            {
                return HttpNotFound(); // error view
            }

            // logic to handle the job application

            return RedirectToAction("ApplicationConfirmation");
        }

        public ActionResult ApplicationConfirmation()
        {
            return View();
        }


        // job employee aka seeker section starts here
    }
}