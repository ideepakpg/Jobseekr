using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jobseekr.Models
{
    [Table("CompanyProfile")]
    public class CompanyProfile
    {
        [Key]
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int Founded { get; set; }
        public long Contact { get; set; }
        public string Website { get; set; }
    }
}