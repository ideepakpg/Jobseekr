﻿using Jobseekr.Models;
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
                    var admin = dbContext.admin.FirstOrDefault(u => u.Username == model.Username);


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
                    else if (admin != null && admin.Password == model.Password)
                    {
                        Session["UserRole"] = admin.Role;
                        Session["AdminId"] = admin.Id;
                        Session["EmployeeId"] = null;
                        Session["EmployerId"] = null;
                        return RedirectToAction("AdminPage");
                    }
                    //else if (user.Role == "Admin")
                    //{
                    //    // Redirect to Admin page
                    //    return RedirectToAction("AdminPage");
                    //}


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
                        //Password = employeeRegistrations.Password,
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
                        //Password = employerRegistration.Password,
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
                // get the currently logged-in employer's ID
                int? employerId = Session["EmployerId"] as int?;

                if (employerId.HasValue)
                {
                    // Set the employer ID in the job listing object
                    jobListings.EmployerId = employerId.Value;

                    // Save the job listing to the database
                    using (var dbContext = new JobseekrDBContext())
                    {
                        // Add the job listing to the database
                        dbContext.jobListings.Add(jobListings);
                        dbContext.SaveChanges();
                    }

                    return RedirectToAction("WelcomePage");
                }
                else
                {
                    // Handle the case where EmployerId is not present in the session
                    return RedirectToAction("Login");
                }
            }

            return View(jobListings);
        }


        public ActionResult Edit(int id)
        {
            using (var dbContext = new JobseekrDBContext())
            {
                // get the job listing by ID
                var jobListing = dbContext.jobListings.Find(id);

                if (jobListing == null)
                {
                    return HttpNotFound();
                }

                // Access the currently logged-in employer's ID
                int? employerId = Session["EmployerId"] as int?;

                if (employerId.HasValue && employerId.Value == jobListing.EmployerId)
                {
                    return View(jobListing);
                }
                else
                {
                    // Set a TempData to indicate unauthorized edit (to show an bootstrap alert message while an employer tries to edit a job that not posted by himself/herself). Fixes issue 36
                    TempData["NotAuthorizedEdit"] = true;
                    return RedirectToAction("WelcomePage"); // show an alert also
                }
            }
        }

        [HttpPost]
        public ActionResult Edit(JobListing jobListing)
        {
            if (ModelState.IsValid)
            {
                using (var dbContext = new JobseekrDBContext())
                {
                    // Check if the logged-in employer is the owner of the job listing
                    int? employerId = Session["EmployerId"] as int?;
                    if (employerId.HasValue && employerId.Value == jobListing.EmployerId)
                    {
                        dbContext.Entry(jobListing).State = EntityState.Modified;
                        dbContext.SaveChanges();
                        return RedirectToAction("WelcomePage");
                    }
                    else
                    {
                        // Set a TempData to indicate unauthorized edit (to show an bootstrap alert message while an employer tries to edit a job that not posted by himself/herself). Fixes issue 36
                        TempData["NotAuthorizedEdit"] = true;
                        return RedirectToAction("WelcomePage");
                    }
                }
            }

            return View(jobListing);
        }


        // this action is to display details of jobs
        public ActionResult Details(int id)
        {
            using (var dbContext = new JobseekrDBContext())
            {
                // Retrieve the job listing by ID
                var jobListing = dbContext.jobListings.Find(id);

                if (jobListing == null)
                {
                    return HttpNotFound();
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
                    return HttpNotFound();
                }

                // Access the currently logged-in employer's ID
                int? employerId = Session["EmployerId"] as int?;

                if (employerId.HasValue && employerId.Value == jobListing.EmployerId)
                {
                    return View(jobListing);
                }
                else
                {
                    // Set a TempData to indicate unauthorized delete (to show an bootstrap alert message while an employer tries to delete a job that not posted by himself/herself). Fixes issue 36
                    TempData["NotAuthorizedDelete"] = true;
                    return RedirectToAction("WelcomePage");
                }
            }
        }


        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var dbContext = new JobseekrDBContext())
            {
                // Retrieve the job listing by ID
                var jobListing = dbContext.jobListings.Find(id);

                // Check if the logged-in employer is the owner of the job listing
                int? employerId = Session["EmployerId"] as int?;
                if (employerId.HasValue && employerId.Value == jobListing.EmployerId)
                {
                    if (jobListing != null)
                    {
                        dbContext.jobListings.Remove(jobListing);
                        dbContext.SaveChanges();
                    }

                    return RedirectToAction("WelcomePage");
                }
                else
                {
                    // Set a TempData to indicate unauthorized delete (to show an bootstrap alert message while an employer tries to delete a job that not posted by himself/herself). Fixes issue 36
                    TempData["NotAuthorizedDelete"] = true;
                    return RedirectToAction("WelcomePage");
                }
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
                // Access the currently logged-in employer's ID
                int? employerId = Session["EmployerId"] as int?;

                if (employerId.HasValue)
                {
                    // Set the employer ID in the company profile object
                    newProfile.EmployerId = employerId.Value;

                    // Add the new company profile to the database
                    obj.companyProfiles.Add(newProfile);
                    obj.SaveChanges();

                    TempData["Message"] = "Company profile created successfully"; // Use TempData for displaying a success message

                    return RedirectToAction("ViewProfile");
                }
                else
                {
                    // Handle the case where EmployerId is not present in the session
                    return RedirectToAction("Login");
                }
            }

            return View(newProfile);
        }



        // Action to view the company profile (the logged in employer posted companies only)
        public ActionResult ViewProfile()
        {
            // Access the currently logged-in employer's ID
            int? employerId = Session["EmployerId"] as int?;

            if (employerId.HasValue)
            {
                // Fetch only the company profiles submitted by the logged-in employer (by id)
                List<CompanyProfile> profiles = obj.companyProfiles
                    .Where(p => p.EmployerId == employerId.Value)
                    .ToList();

                return View(profiles);
            }
            else
            {
                // Handle the case where EmployerId is not present in the session
                return RedirectToAction("Login");
            }
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

                }

                return RedirectToAction("ViewProfile");
            }

            return View(updatedProfile);
        }

        // this action is to display all companies posted by every employers
        public ActionResult AllCompanyProfiles()
        {
            // Fetch all company profiles from the database
            List<CompanyProfile> profiles = obj.companyProfiles.ToList();

            return View(profiles);
        }



        [HttpGet]
        public ActionResult ViewEnquiries()
        {
            var enquiries = obj.enquiryListings.ToList(); // get all employee enquires from the database.

            return View(enquiries);
        }


        //to view job application applied by the employee . Fixes #32
        public ActionResult ViewJobApplications()
        {
            // Check if the employer is logged in
            int employerId;
            if (Session["EmployerId"] != null && int.TryParse(Session["EmployerId"].ToString(), out employerId))
            {
                // Get the list of job IDs posted by the employer
                var employerJobIds = obj.jobListings.Where(j => j.EmployerId == employerId).Select(j => j.JobId).ToList();

                // Get job applications for the employer's posted jobs
                var jobApplications = obj.jobApplicationListings
                    .Include(ja => ja.JobListing) // Load the associated job details
                    .Where(ja => employerJobIds.Contains(ja.JobId))
                    .ToList();

                return View(jobApplications);
            }
            else
            {
                ViewBag.Message = "Invalid session or employer ID.";
                return View("Error");
            }
        }




        // Action to send response to job application
        public ActionResult SendJobApplicationResponse(int applicationId)
        {
            // get job application from the database based on the applicationId
            using (var dbContext = new JobseekrDBContext())
            {
                var jobApplication = dbContext.jobApplicationListings.Find(applicationId);

                if (jobApplication == null)
                {
                    return HttpNotFound();
                }

                // Pass job application details to the view
                return View(jobApplication);
            }
        }

        // Action to handle the response submission
        [HttpPost]
        public ActionResult SubmitJobApplicationResponse(int applicationId, string response)
        {
            // Update the job application status based on the response
            using (var dbContext = new JobseekrDBContext())
            {
                var jobApplication = dbContext.jobApplicationListings.Find(applicationId);

                if (jobApplication == null)
                {
                    return HttpNotFound();
                }

                // Update the response status (table) in database
                jobApplication.ResponseStatus = response;


                dbContext.SaveChanges();
            }

            // Redirect to the job applications view
            return RedirectToAction("ViewJobApplications");
        }



        public ActionResult EmployerProfile()
        {
            // Check if the user is logged in as an employer
            if (Session["UserRole"] == null || (string)Session["UserRole"] != "Employer")
            {
                // Redirect to the login page or handle the unauthorized access case
                return RedirectToAction("Login", "Jobseekr");
            }

            // get employer's ID from the session
            int employerId;
            if (Session["EmployerId"] != null)
            {
                employerId = (int)Session["EmployerId"];
            }
            else
            {
                return RedirectToAction("Login", "Jobseekr");
            }

            // Get the employee details from the database table (EmployerLogin) to display employer profile
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


        public ActionResult EditEmployerProfile(int Id)
        {
            using (var dbContext = new JobseekrDBContext())
            {
                // Get the employer profile by ID
                var employerProfile = dbContext.employerLoginsforProfile.Find(Id);

                if (employerProfile == null)
                {
                    return HttpNotFound(); // 404
                }

                return View(employerProfile);
            }
        }

        [HttpPost]
        public ActionResult EditEmployerProfile(EmployerLogin employerProfile)
        {
            if (ModelState.IsValid)
            {
                using (var dbContext = new JobseekrDBContext())
                {
                    // Added the code to address the issue of the employer's status (role) getting deleted after updating profile details
                    // in the EditEmployerProfile view, this code retrieves the original EmployerLogin object from the database, updates only the editable fields, and saves the changes. This ensures that the status (role) remains unchanged during the profile update operation.
                    // Fixes issue #22

                    // Retrieve the original employee object from the database
                    var originalEmployer = dbContext.employerLoginsforProfile.Find(employerProfile.Id);

                    if (originalEmployer == null)
                    {
                        return HttpNotFound(); // 404
                    }

                    // Update only the editable fields (enn vecha status role update cheyyaruth - update cheytha status role in EmployerProfile will get lost)
                    originalEmployer.FirstName = employerProfile.FirstName;
                    originalEmployer.LastName = employerProfile.LastName;
                    originalEmployer.EmailId = employerProfile.EmailId;
                    originalEmployer.Username = employerProfile.Username;
                    originalEmployer.MobileNumber = employerProfile.MobileNumber;

                    dbContext.SaveChanges();

                    // Update EmployerRegistration if it has the same Id of employer
                    var employerRegistration = dbContext.employerRegistrations.FirstOrDefault(e => e.Id == employerProfile.Id);
                    if (employerRegistration != null)
                    {
                        employerRegistration.FirstName = employerProfile.FirstName;
                        employerRegistration.LastName = employerProfile.LastName;
                        employerRegistration.EmailId = employerProfile.EmailId;
                        employerRegistration.Username = employerProfile.Username;
                        //employerRegistration.Password = employerProfile.Password;
                        employerRegistration.MobileNumber = employerProfile.MobileNumber;
                    }

                    // Update LoginEmployer if it has the same Id of employer
                    var loginEmployer = dbContext.loginsofEmployers.FirstOrDefault(e => e.Id == employerProfile.Id);
                    if (loginEmployer != null)
                    {
                        loginEmployer.Username = employerProfile.Username;
                        //loginEmployer.Password = employerProfile.Password;
                    }

                    dbContext.SaveChanges();
                }

                return RedirectToAction("EmployerProfile");
            }

            return View(employerProfile);
        }


        // to view reviews of employees for the jobs employers posted . Fixes #31
        public ActionResult ViewEmployeeReviews(int? jobId)
        {
            // Check if the employer is logged in
            int employerId;
            if (Session["EmployerId"] != null && int.TryParse(Session["EmployerId"].ToString(), out employerId))
            {
                // Get the list of job IDs posted by the employer
                var employerJobIds = obj.jobListings.Where(j => j.EmployerId == employerId).Select(j => j.JobId).ToList();

                // Get reviews for the specified job ID and employer's posted jobs
                var jobReviews = obj.reviews
                    .Where(r => employerJobIds.Contains(r.JobId))
                    .ToList();

                return View(jobReviews);
            }
            else
            {
                ViewBag.Message = "Invalid session or employer ID.";
                return View("Error");
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
            // Get the authenticated employee's Id from the session
            int? employeeId = Session["EmployeeId"] as int?;

            if (employeeId.HasValue)
            {
                JobApplication application = new JobApplication { JobId = jobId, EmployeeId = employeeId.Value };

                // Fetch the associated JobListing to get the EmployerId
                using (var dbContext = new JobseekrDBContext())
                {
                    var jobListing = dbContext.jobListings.Find(jobId);

                    // Ensure the JobListing is found
                    if (jobListing != null)
                    {
                        // Set the EmployerId property in the JobApplication
                        application.EmployerId = jobListing.EmployerId;

                        return View("SubmitApplication", application);
                    }
                    else
                    {
                        // handle the case where the associated JobListing is not found
                        return RedirectToAction("Error");
                    }
                }
            }
            else
            {
                // Redirect to login page
                return RedirectToAction("Login");
            }
        }


        // this action is to submit the job application by the employees
        [HttpPost]
        public ActionResult SubmitApplication(JobApplication application)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the employee ID from the session
                int? employeeId = Session["EmployeeId"] as int?;

                // Check if the employeeId is available
                if (employeeId.HasValue)
                {
                    // Check if the employee has already applied for the specified job
                    using (var dbContext = new JobseekrDBContext())
                    {
                        bool hasApplied = dbContext.jobApplicationListings
                            .Any(app => app.JobId == application.JobId && app.EmployeeId == employeeId);

                        if (hasApplied)
                        {
                            // Set a TempData to indicate mutliple job applies (to show an bootstrap alert message when employee tries to apply for a job that he/she already applied). Fixes issue 34
                            TempData["EnoughApply"] = true;
                            return RedirectToAction("AvailableJobs");
                        }

                        // Fetch the associated JobListing to get the EmployerId
                        var jobListing = dbContext.jobListings.Find(application.JobId);

                        // Ensure the JobListing is found
                        if (jobListing != null)
                        {
                            // Set the EmployerId and EmployeeId properties in the JobApplication to store in the database
                            application.EmployerId = jobListing.EmployerId;
                            application.EmployeeId = employeeId.Value;

                            // Set the JobTitle property in the JobApplication
                            application.JobTitle = jobListing.JobTitle;

                            // Set a default text in ResponseStatus; after the employer selects selected/rejected, it will be removed and updated
                            application.ResponseStatus = "Pending";

                            dbContext.jobApplicationListings.Add(application);
                            dbContext.SaveChanges();

                            return RedirectToAction("ApplicationConfirmation");
                        }
                        else
                        {
                            return RedirectToAction("Error");
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }

            return View(application);
        }




        public ActionResult ApplicationConfirmation()
        {
            return View();
        }


        public ActionResult ViewMyJobApplicationStatus()
        {
            // Get the EmployeeId from the session
            int? employeeId = Session["EmployeeId"] as int?;

            if (!employeeId.HasValue)
            {
                return RedirectToAction("Login");
            }

            using (var dbContext = new JobseekrDBContext())
            {
                // Retrieve job applications submitted by the employee along with job details
                var jobApplications = dbContext.jobApplicationListings
                    .Include(j => j.JobListing) // Load the associated job details
                    .Where(j => j.EmployeeId == employeeId.Value)
                    .ToList();

                return View(jobApplications);
            }
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
                // Access the currently logged-in employee's ID
                int? employeeId = Session["EmployeeId"] as int?;

                if (employeeId.HasValue)
                {
                    // Set the employee ID in the enquiry object
                    enquiry.EmployeeId = employeeId.Value;

                    // Save the enquiry to the database
                    obj.enquiryListings.Add(enquiry);
                    obj.SaveChanges();

                    // Submission was successful
                    return Json(new { success = true });
                }
                else
                {
                    // Handle the case where EmployeeId is not present in the session
                    // (e.g., redirect to login or display an error message)
                    return Json(new { success = false, error = "EmployeeId not found in session." });
                }
            }

            // Submission failed
            return Json(new { success = false });
        }




        public ActionResult EmployeeInbox()
        {
            // Access the currently logged-in employee's ID
            int? employeeId = Session["EmployeeId"] as int?;

            if (employeeId.HasValue)
            {
                // Fetch only the enquiries submitted by the logged-in employee (based on EmployeeId)
                List<Enquiry> enquiries = obj.enquiryListings
                    .Where(e => e.EmployeeId == employeeId.Value)
                    .ToList();

                return View(enquiries);
            }
            else
            {
                // Handle the case where EmployeeId is not present in the session
                return RedirectToAction("Login");
            }
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

            // get employee's ID from the session
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

            // Get the employee details from the database table (EmployeeLogin) to display employee profile
            using (var db = new JobseekrDBContext())
            {
                var employee = db.employeeLoginsforProfile.FirstOrDefault(e => e.Id == employeeId);

                if (employee == null)
                {
                    // Handle the case where the employee ID is not found in the database
                    return HttpNotFound(); // show an error  
                }

                return View(employee);
            }
        }


        public ActionResult EditEmployeeProfile(int? Id)
        {
            using (var dbContext = new JobseekrDBContext())
            {
                // Get the employee profile by ID
                var employeeProfile = dbContext.employeeLoginsforProfile.Find(Id);

                if (employeeProfile == null)
                {
                    return HttpNotFound(); // 404
                }

                return View(employeeProfile);
            }
        }

        [HttpPost]
        public ActionResult EditEmployeeProfile(EmployeeLogin employeeProfile)
        {
            if (ModelState.IsValid)
            {
                using (var dbContext = new JobseekrDBContext())
                {
                    // Added tje code to address the issue of the employee's status (role) getting deleted after updating profile details
                    // in the EditEmployeeProfile view, this code retrieves the original EmployeeLogin object from the database, updates only the editable fields, and saves the changes. This ensures that the status (role) remains unchanged during the profile update operation.
                    // Fixes issue #21

                    // Retrieve the original employee object from the database
                    var originalEmployee = dbContext.employeeLoginsforProfile.Find(employeeProfile.Id);

                    if (originalEmployee == null)
                    {
                        return HttpNotFound(); // show error
                    }

                    // Update only the editable fields (enn vecha status role update cheyyaruth - update cheytha status role in EmployeeProfile will get lost)
                    originalEmployee.FirstName = employeeProfile.FirstName;
                    originalEmployee.LastName = employeeProfile.LastName;
                    originalEmployee.EmailId = employeeProfile.EmailId;
                    originalEmployee.Username = employeeProfile.Username;
                    originalEmployee.MobileNumber = employeeProfile.MobileNumber;

                    dbContext.SaveChanges();

                    // Update EmployeeRegistration if it has the same Id of employee
                    var employeeRegistration = dbContext.employeeRegistrations.FirstOrDefault(e => e.Id == employeeProfile.Id);
                    if (employeeRegistration != null)
                    {
                        employeeRegistration.FirstName = employeeProfile.FirstName;
                        employeeRegistration.LastName = employeeProfile.LastName;
                        employeeRegistration.EmailId = employeeProfile.EmailId;
                        employeeRegistration.Username = employeeProfile.Username;
                        //employeeRegistration.Password = employeeProfile.Password;
                        employeeRegistration.MobileNumber = employeeProfile.MobileNumber;

                    }

                    // Update LoginEmployee if it has the same Id of employee
                    var loginEmployee = dbContext.loginsofEmployees.FirstOrDefault(e => e.Id == employeeProfile.Id);
                    if (loginEmployee != null)
                    {
                        loginEmployee.Username = employeeProfile.Username;
                        //loginEmployee.Password = employeeProfile.Password;
                    }

                    dbContext.SaveChanges();
                }

                return RedirectToAction("EmployeeProfile");
            }

            return View(employeeProfile);
        }




        public ActionResult ReviewJob(int jobId)
        {
            // Check if the employee has already reviewed the job
            int employeeId;
            if (Session["EmployeeId"] != null && int.TryParse(Session["EmployeeId"].ToString(), out employeeId))
            {
                bool hasReviewed = obj.reviews.Any(r => r.JobId == jobId && r.EmployeeId == employeeId);

                if (hasReviewed)
                {
                    ViewBag.Message = "You have already reviewed this job.";
                    return View("ReviewError");
                }
            }
            else
            {
                ViewBag.Message = "Invalid session or employee ID.";
                return View("ReviewError");
            }

            // get the job you want to review
            JobListing jobToReview = obj.jobListings.Find(jobId);

            if (jobToReview == null)
            {
                return HttpNotFound(); // 404
            }

            return View(jobToReview);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReviewJob(Review review)
        {
            if (ModelState.IsValid)
            {
                // Check if the employee has already reviewed the job
                int employeeId;
                if (Session["EmployeeId"] != null && int.TryParse(Session["EmployeeId"].ToString(), out employeeId))
                {
                    bool hasReviewed = obj.reviews.Any(r => r.JobId == review.JobId && r.EmployeeId == employeeId);

                    if (hasReviewed)
                    {
                        ViewBag.Message = "You have already reviewed this job.";
                        return View("ReviewError");
                    }

                    // Get the job you want to review
                    JobListing jobToReview = obj.jobListings.Find(review.JobId);

                    if (jobToReview == null)
                    {
                        return HttpNotFound(); // 404
                    }

                    // Save the review and ratings to the database
                    review.EmployeeId = employeeId; // Set the employee ID in the review
                    review.EmployerId = jobToReview.EmployerId; // Set the employer ID in the review
                    obj.reviews.Add(review);
                    obj.SaveChanges();

                    ViewBag.Message = "Thank you for your review!";
                    return View("ReviewSuccess");
                }
                else
                {
                    ViewBag.Message = "Invalid session or employee ID.";
                    return View("Error");
                }
            }

            return View(review);
        }



        // job employee aka seeker section ends here




        // admin section starts here

        public ActionResult AdminPage()
        {
            return View();
        }


        public ActionResult ViewAllEmployees()
        {
            // Fetch all employees details from the database
            List<EmployeeRegistration> employees = obj.employeeRegistrations.ToList();

            // Pass the list of employees to the view
            return View(employees);
        }


        public ActionResult ViewAllEmployers()
        {
            // Fetch all employers details from the database
            List<EmployerRegistration> employers = obj.employerRegistrations.ToList();

            // Pass the list of employers to the view
            return View(employers);
        }

        public ActionResult ViewAllCompanyProfiles()
        {
            // Fetch all employers details from the database
            List<CompanyProfile> companies = obj.companyProfiles.ToList();

            // Pass the list of employers to the view
            return View(companies);
        }

        public ActionResult ViewAllJobs()
        {
            // Fetch all employers details from the database
            List<JobListing> jobs = obj.jobListings.ToList();

            // Pass the list of employers to the view
            return View(jobs);
        }

        public ActionResult DeleteJob(int jobId)
        {
            var jobToDelete = obj.jobListings.FirstOrDefault(j => j.JobId == jobId);

            if (jobToDelete != null)
            {
                obj.jobListings.Remove(jobToDelete);
                obj.SaveChanges();
            }

            // Redirect back to the view
            return RedirectToAction("ViewAllJobs");
        }

        public ActionResult ViewAllJobApplications()
        {
            // Fetch all employers details from the database
            List<JobApplication> jobapplications = obj.jobApplicationListings.ToList();

            // Pass the list of employers to the view
            return View(jobapplications);
        }

        public ActionResult ViewAllEnquiries()
        {
            // Fetch all employers details from the database
            List<Enquiry> enquiries = obj.enquiryListings.ToList();

            // Pass the list of employers to the view
            return View(enquiries);
        }

        [HttpPost]
        public ActionResult ViewAllEnquiries(int enquiryId, string adminReply)
        {
            Enquiry enquiry = obj.enquiryListings.Find(enquiryId);

            if (enquiry != null)
            {
                enquiry.Reply = adminReply;
                obj.SaveChanges();
            }

            return Json(new { success = true });
        }



        public ActionResult ViewAllReviewsandRatings()
        {
            // Fetch all employers details from the database
            List<Review> reviews = obj.reviews.ToList();

            // Pass the list of employers to the view
            return View(reviews);
        }





        // admin section ends here
    }
}