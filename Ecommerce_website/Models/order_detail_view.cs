using System.ComponentModel.DataAnnotations;

namespace Ecommerce_website.Models
{
    public class order_detail_view
    {
       
            [Key]  // 👈 Primary key for EF
        public int order_id { get; set; }
        public string product_name { get; set; }
        public int quantity { get; set; }
        public string order_status { get; set; }
        public int total { get; set; }

        public int customer_id { get; set; }
        public string ord_name { get; set; }
        public string ord_email { get; set; }
        public long ord_phone { get; set; }
        public string state_name { get; set; }
        public string ord_address { get; set; }



    }
}
