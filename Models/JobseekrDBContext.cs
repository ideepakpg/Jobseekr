﻿using System.Data.Entity;

namespace Jobseekr.Models
{
    public class JobseekrDBContext : DbContext
    {
        public DbSet<Login> logins { get; set; }
        public DbSet<EmployeeRegistration> employeeRegistrations { get; set; }
        public DbSet<JobListing> jobListings { get; set; }
        public DbSet<CompanyProfile> companyProfiles { get; set; }
        public DbSet<Enquiry> enquiryListings { get; set; }
        public DbSet<JobApplication> jobApplicationListings { get; set; }
        public DbSet<EmployerRegistration> employerRegistrations { get; set; }
        public DbSet<EmployeeLogin> employeeLoginsforProfile { get; set; }
    }
}