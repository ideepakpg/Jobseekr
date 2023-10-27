using System.Data.Entity;

namespace Jobseekr.Models
{
    public class JobseekrDBContext : DbContext
    {
        public DbSet<EmployeeLogin> employeeLogins { get; set; }
        public DbSet<EmployeeRegistration> employeeRegistrations { get; set; }
        public DbSet<JobListing> jobListings { get; set; }
    }
}