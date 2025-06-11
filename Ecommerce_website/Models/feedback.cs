using System.ComponentModel.DataAnnotations;

namespace Ecommerce_website.Models
{
    public class feedback
    {
        [Key]
       public int feedback_id { get; set; }

       public string user_name { get; set; }

        public string message { get; set; }
    }
}
