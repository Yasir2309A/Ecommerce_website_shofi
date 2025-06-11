using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_website.Models
{
    public class product
    {
        [Key]
        public int product_id { get; set; }

        [Required]
        public string product_name { get; set; }

        [Required]
        public int product_price { get; set; }

        [Required]
        public string product_description { get; set; }

        [Required]
        public string product_image { get; set; }

        [Required]
        public int product_quantity { get; set; }

        [Required]
        public int cat_id { get; set; }

        [ForeignKey("cat_id")]
        public virtual catogery Catogery { get; set; }
    }
}
