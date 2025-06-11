using System.ComponentModel.DataAnnotations;

namespace Ecommerce_website.Models
{
    public class State
    {
        [Key]
        public int state_id { get; set; }

        [Required]
        public string state_name { get; set; }
    }
}
