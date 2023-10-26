using System.Data.Entity;

namespace Jobseekr.Models
{
    public class JobseekrDBContext : DbContext
    {
        public DbSet<EmployeeLogin> employeeLogins { get; set; }
    }
}