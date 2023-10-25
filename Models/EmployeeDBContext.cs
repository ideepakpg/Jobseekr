using System.Data.Entity;

namespace Jobseekr.Models
{
    public class EmployeeDBContext : DbContext
    {
        public DbSet<EmployeeLogin> employeeLogins { get; set; }
    }
}