using Jobseekr.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Jobseekr.Controllers
{
    public class JobseekrController : Controller
    {
        private JobseekrDBContext obj = new JobseekrDBContext();

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

        // Action to handle employee and employer login
        [HttpGet]
        public ActionResult Login()
        {
            var model = new LoginCommon(); // Create an instance of the common login model
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginCommon model)
        {
            if (ModelState.IsValid)
            {
                using (var dbContext = new JobseekrDBContext())
                {
                    var employee = dbContext.loginsofEmployees.FirstOrDefault(u => u.Username == model.Username);
                    var employer = dbContext.loginsofEmployers.FirstOrDefault(u => u.Username == model.Username);


                    if (employee != null && employee.Password == model.Password)
                    {
                        Session["UserRole"] = employee.Role;
                        Session["EmployeeId"] = employee.Id;
                        Session["EmployerId"] = null;
                        return RedirectToAction("AvailableJobs");
                    }
                    else if (employer != null && employer.Password == model.Password)
                    {
                        Session["UserRole"] = employer.Role;
                        Session["EmployerId"] = employer.Id;
                        Session["EmployeeId"] = null;
                        return RedirectToAction("WelcomePage");
                    }
                    //else if (user.Role == "Admin")
                    //{
                    //    // Redirect to Admin page
                    //    return RedirectToAction("AdminPage");
                    //}
                    //return RedirectToAction("WelcomePage");


                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password.");
                    }
                }
            }
            ViewBag.ShowErrorMessage = true;
            return View(model);
        }



        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("LandingPage");
        }


        // Action for Employee registration
        [HttpGet]
        public ActionResult Registration()
        {

            EmployeeRegistration obj2 = new EmployeeRegistration();
            obj.employeeRegistrations.Add(obj2);
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

                    var loginemployee = new LoginEmployee
                    {
                        Username = employeeRegistrations.Username,
                        Password = employeeRegistrations.Password,
                        Role = "Employee"
                    };

                    var employeeLogin = new EmployeeLogin
                    {
                        FirstName = employeeRegistrations.FirstName,
                        LastName = employeeRegistrations.LastName,
                        EmailId = employeeRegistrations.EmailId,
                        Username = employeeRegistrations.Username,
                        Password = employeeRegistrations.Password,
                        MobileNumber = employeeRegistrations.MobileNumber,
                        Role = "Employee"
                    };

                    // Add and save the instances to their respective tables
                    dbContext.employeeRegistrations.Add(registration);
                    dbContext.loginsofEmployees.Add(loginemployee);
                    dbContext.employeeLoginsforProfile.Add(employeeLogin);
                    dbContext.SaveChanges();
                }

                return RedirectToAction("RegistrationSuccess");
            }

            return View();
        }

        // Registration for Employer 
        [HttpGet]
        public ActionResult EmployerRegistration()
        {
            EmployerRegistration obj = new EmployerRegistration();
            return View(obj);
        }

        [HttpPost]
        public ActionResult EmployerRegistration(EmployerRegistration employerRegistration)
        {
            if (ModelState.IsValid)
            {
                using (var dbContext = new JobseekrDBContext())
                {
                    // Create an instance for the employer registration
                    var registration = new EmployerRegistration
                    {
                        FirstName = employerRegistration.FirstName,
                        LastName = employerRegistration.LastName,
                        EmailId = employerRegistration.EmailId,
                        Username = employerRegistration.Username,
                        Password = employerRegistration.Password,
                        MobileNumber = employerRegistration.MobileNumber
                    };

                    // Add the employer registration to the EmployerRegistration table
                    dbContext.employerRegistrations.Add(registration);
                    dbContext.SaveChanges();

                    // instance for store employer details to the EmployerLogin table
                    var employerLogin = new EmployerLogin
                    {
                        FirstName = employerRegistration.FirstName,
                        LastName = employerRegistration.LastName,
                        EmailId = employerRegistration.EmailId,
                        MobileNumber = employerRegistration.MobileNumber,
                        Username = employerRegistration.Username,
                        Password = employerRegistration.Password,
                        Role = "Employer"
                    };

                    // Add the login information to the EmployerLogin table
                    dbContext.employerLoginsforProfile.Add(employerLogin);
                    dbContext.SaveChanges();


                    // Create an instance for the login
                    var loginemployer = new LoginEmployer
                    {
                        Username = employerRegistration.Username,
                        Password = employerRegistration.Password,
                        Role = "Employer"
                    };

                    // Add the login information to the EmployeeLogin table
                    dbContext.loginsofEmployers.Add(loginemployer);
                    dbContext.SaveChanges();


                }

                return RedirectToAction("RegistrationSuccess");
            }

            return View(employerRegistration);
        }




        public ActionResult RegistrationSuccess()
        {

            return View();
        }

        // job provider aka employer section starts here


        public ActionResult WelcomePage()  /*for employer*/
        {
            var jobListings = obj.jobListings.ToList();
            return View(jobListings);
        }


        //Available Jobs CRUD operations
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

        // Company profile management

        [HttpGet]
        public ActionResult CreateProfile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateProfile(CompanyProfile newProfile)
        {
            if (ModelState.IsValid)
            {
                // Add the new company profile to the database
                obj.companyProfiles.Add(newProfile);
                obj.SaveChanges();

                TempData["Message"] = "Company profile created successfully"; // Use TempData for displaying a success message

                return RedirectToAction("ViewProfile");
            }

            return View(newProfile);
        }


        // Action to view the company profile
        public ActionResult ViewProfile()
        {
            // Fetch all company profiles from the database
            List<CompanyProfile> profiles = obj.companyProfiles.ToList();

            return View(profiles);
        }


        // Action to edit the company profile
        [HttpGet]
        public ActionResult EditProfile(int id)
        {
            // Retrieve the company profile from the database using the provided ID
            CompanyProfile profile = obj.companyProfiles.Find(id);

            if (profile == null)
            {
                return HttpNotFound(); // Handle the case where the profile is not found
            }

            return View(profile);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(CompanyProfile updatedProfile)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the existing profile from the database using the updated profile's ID
                CompanyProfile existingProfile = obj.companyProfiles.Find(updatedProfile.CompanyId);

                if (existingProfile != null)
                {
                    // Update all properties of the existing profile
                    existingProfile.CompanyName = updatedProfile.CompanyName;
                    existingProfile.Description = updatedProfile.Description;
                    existingProfile.Location = updatedProfile.Location;
                    existingProfile.Founded = updatedProfile.Founded;
                    existingProfile.Contact = updatedProfile.Contact;
                    existingProfile.Website = updatedProfile.Website;

                    // Save changes to the database
                    obj.SaveChanges();

                    TempData["Message"] = "Profile updated successfully"; // Use TempData for displaying a success message
                }

                return RedirectToAction("ViewProfile");
            }

            return View(updatedProfile);
        }


        [HttpGet]
        public ActionResult ViewEnquiries()
        {
            var enquiries = obj.enquiryListings.ToList(); // get all employee enquires from the database.

            return View(enquiries);
        }


        //to view job application applied by the employee
        public ActionResult ViewJobApplications()
        {
            // Retrieve job applications from database
            using (var dbContext = new JobseekrDBContext())
            {
                var jobApplications = dbContext.jobApplicationListings.ToList();
                return View(jobApplications);
            }
        }


        public ActionResult EmployerProfile()
        {
            if (Session["UserRole"] == null || (string)Session["UserRole"] != "Employer")
            {
                return RedirectToAction("Login", "Jobseekr");
            }

            int employerId;
            if (Session["EmployerId"] != null)
            {
                employerId = (int)Session["EmployerId"];
            }
            else
            {
                return RedirectToAction("Login", "Jobseekr");
            }

            using (var db = new JobseekrDBContext())
            {
                var employer = db.employerLoginsforProfile.FirstOrDefault(e => e.Id == employerId);

                if (employer == null)
                {
                    return HttpNotFound();
                }

                return View(employer);
            }
        }






        // job provider aka employer section ends here


        // job employee aka seeker section starts here


        public ActionResult AvailableJobs()
        {
            var jobListings = obj.jobListings.ToList();
            return View(jobListings);
        }

        public ActionResult ApplyJob(int jobId)
        {
            JobApplication application = new JobApplication { JobId = jobId };
            return View("SubmitApplication", application);
        }


        [HttpPost]
        public ActionResult SubmitApplication(JobApplication application)
        {
            if (ModelState.IsValid)
            {
                using (var dbContext = new JobseekrDBContext())
                {
                    dbContext.jobApplicationListings.Add(application);
                    dbContext.SaveChanges();
                }

                return RedirectToAction("ApplicationConfirmation");
            }

            return View(application);
        }



        public ActionResult ApplicationConfirmation()
        {
            return View();
        }

        public ActionResult ViewCompanyProfiles()
        {
            var profiles = obj.companyProfiles.ToList(); // Retrieve all company profiles
            return View(profiles);
        }

        public ActionResult SearchJobs(string jobName, string cityName)   // view is in AvailableJobs.cshtml
        {
            IEnumerable<Jobseekr.Models.JobListing> jobListings;

            if (!string.IsNullOrEmpty(jobName) || !string.IsNullOrEmpty(cityName))
            {
                // If search criteria is provided, filter job listings
                jobListings = obj.jobListings
                    .Where(j => (string.IsNullOrEmpty(jobName) || j.JobTitle.Contains(jobName))
                                && (string.IsNullOrEmpty(cityName) || j.Location.Contains(cityName)))
                    .ToList();
            }
            else
            {
                // If no search criteria is provided, show all available jobs
                jobListings = obj.jobListings.ToList();
            }

            return View("AvailableJobs", jobListings);
        }

        [HttpGet]
        public ActionResult Enquiry()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Enquiry(Enquiry enquiry)
        {
            if (ModelState.IsValid)
            {
                obj.enquiryListings.Add(enquiry);
                obj.SaveChanges();
                // Submission was successful
                return Json(new { success = true });
            }

            // Submission failed
            return Json(new { success = false });
        }


        // Action to display the employee profile
        public ActionResult EmployeeProfile()
        {
            // Check if the user is logged in as an employee
            if (Session["UserRole"] == null || (string)Session["UserRole"] != "Employee")
            {
                // Redirect to the login page or handle the unauthorized access case
                return RedirectToAction("Login", "Jobseekr");
            }

            // Retrieve the employee's ID from the session
            int employeeId;
            if (Session["EmployeeId"] != null)
            {
                employeeId = (int)Session["EmployeeId"];
            }
            else
            {
                // Handle the case where the employee ID is not found in the session
                return RedirectToAction("Login", "Jobseekr"); // Redirect to the login page
            }

            // Get the employee details from the database
            using (var db = new JobseekrDBContext())
            {
                var employee = db.employeeLoginsforProfile.FirstOrDefault(e => e.Id == employeeId);

                if (employee == null)
                {
                    // Handle the case where the employee ID is not found in the database
                    return HttpNotFound(); // Return a 404 Not Found status
                }

                return View(employee);
            }
        }


        // job employee aka seeker section starts here
    }
}