using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TravelAgentWeb.Data;
using TravelAgentWeb.Dtos;

namespace TravelAgentWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase {

    private readonly TravelAgentDbContext _context;
     public NotificationsController(TravelAgentDbContext context)
     {
         _context = context;
     }

     [HttpPost]
     public ActionResult FlightChanged(FlightDetailUpdateDto flightDetailUpdateDto){

         Console.WriteLine($"Webhook Received from: {flightDetailUpdateDto.Publisher}");

         var secretModel = _context.WebhoookSecrets.FirstOrDefault(x => 
         x.Publisher == flightDetailUpdateDto.Publisher && 
         x.Secret == flightDetailUpdateDto.Secret);

         if (secretModel == null)
         {
             Console.ForegroundColor = ConsoleColor.Red;
             Console.WriteLine("Invalid Secret - Ignore Webhook");
             Console.ResetColor();
             return Ok();
         }
         else
         {
             Console.ForegroundColor = ConsoleColor.Green;
             Console.WriteLine("Valid webhook");
             Console.WriteLine($"Old Price {flightDetailUpdateDto.OldPrice}, New Price {flightDetailUpdateDto.NewPrice}");
             Console.ResetColor();
             return Ok();
         }
     }

    }    
}