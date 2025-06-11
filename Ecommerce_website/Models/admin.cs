using System.ComponentModel.DataAnnotations;

namespace Ecommerce_website.Models
{
    public class admin
    {
        [Key]
        public int admin_id { get; set; }

        [Required]
        public string admin_name { get; set; }


        [Required]
        [RegularExpression("[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}", ErrorMessage = "Enter Invalid Email")]
        public string admin_email { get; set; }


        [Required]
        [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{5,}", ErrorMessage = "Password must be strong")]
        public string admin_password { get; set; }

        public string admin_image { get; set; }
    }
}
