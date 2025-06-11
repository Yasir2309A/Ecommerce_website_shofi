using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_website.Models
{
    public class cart
    {
        [Key]
        public int cart_id { get; set; }

       
        public int product_id { get; set; }

        public int customer_id { get; set; }

        public int quantity { get; set; }

        public int total_price { get; set; }    

        public string status { get; set; }

        [ForeignKey("product_id")]
        public product products { get; set; }

        [ForeignKey("customer_id")]
        public customer customers { get; set; }


    }
}
