using System.ComponentModel.DataAnnotations;

namespace Ecommerce_website.Models
{
    public class catogery
    {
        [Key]
        public int cat_id { get; set; }

        [Required]
        public string cat_name { get; set; }
    }
}
