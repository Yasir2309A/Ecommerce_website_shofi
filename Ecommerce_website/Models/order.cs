using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json.Serialization;

namespace Ecommerce_website.Models
{
    public class order
    {
        [Key]
        public int order_id { get; set; }

        public int customer_id { get; set; }

        [Required(ErrorMessage = "Please enter your name.")]
        public string ord_name { get; set; }

        [Required(ErrorMessage = "Please enter your email.")]
        public string ord_email { get; set; }

        [Required(ErrorMessage = "Please enter your phone.")]

        public long ord_phone { get; set; }
        [Required(ErrorMessage = "Please enter your state.")]
        public int state_id { get; set; }

        [Required(ErrorMessage = "Please enter your address.")]
        public string ord_address { get; set; }


        [ForeignKey("customer_id")]
        public customer customers { get; set; }


        [ForeignKey("state_id")]
        public State state { get; set; }
        
    }
}
