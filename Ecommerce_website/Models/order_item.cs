using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_website.Models
{
    public class order_item
    {
        [Key]
        public int order_item_id { get; set; }
        public int order_id { get; set; }
        public int product_id { get; set; }

        public int quantity { get; set; }

        public string order_status { get; set; }
        public int total { get; set; }

        [ForeignKey("order_id")]
        public order orders { get; set; }

        [ForeignKey("product_id")]
        public product products { get; set; }
    }
}
