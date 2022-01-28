using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace  AirlineWeb.Models 
{
    public class FlightDetail {

        [Required]
        public int Id { get; set; }
        [Required]
        public string FlightCode { get; set; }
        [Required]
        [Column(TypeName = "decimal(6,2)")]
        public decimal Price {get; set;}       
        
    }
}