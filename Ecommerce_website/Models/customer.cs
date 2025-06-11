using System.ComponentModel.DataAnnotations;
using NuGet.DependencyResolver;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_website.Models
{
    public class customer
    {
        [Key]
        public int customer_id { get; set; }

        [Required]
        public string customer_name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string customer_email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string coustomer_password { get; set; }

        [Required]
        public long customer_phone { get; set; }

        public string Country { get; set; }

        [Required]
        public int state_id { get; set; }

        [Required]
        public string address { get; set; }

        [ForeignKey("state_id")]
        public virtual State state { get; set; }

        [NotMapped]
        public string VerificationCode { get; set; }
    }
}
