using System;
using System.Linq;
using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AirlineWeb.MessageBus;
using AirlineWeb.Models;
using AutoMapper;

using Microsoft.AspNetCore.Mvc;

namespace AirlineWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase {

        private readonly AirlineDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBus;

        public FlightsController(AirlineDbContext context, IMapper mapper, IMessageBusClient messageBus){
            _context = context;
            _mapper = mapper;
            _messageBus = messageBus;
        }
       
       [HttpGet("{flightCode}", Name = "GetFligthDetailsByCode")]
       public ActionResult<FlightDetailReadDto> GetFligthDetailsByCode(string flightCode){

           var filght = _context.FlightDetails.FirstOrDefault(x => x.FlightCode == flightCode);

           if (filght == null) {
               
               return NotFound();
           }

           return Ok(_mapper.Map<FlightDetailReadDto>(filght));
       }

        [HttpPost]
       public ActionResult<FlightDetailReadDto> CreateFlight(FlightDetailCreateDto flightDetailCreteDto){

           var flight = _context.FlightDetails.FirstOrDefault(x => x.FlightCode == flightDetailCreteDto.FlightCode);

           if (flight == null ) {
               var flightDetailModel = _mapper.Map<FlightDetail>(flightDetailCreteDto);
               try
               {
                   _context.FlightDetails.Add(flightDetailModel);
                   _context.SaveChanges();
               }
               catch (Exception ex)
               {
                   return BadRequest(ex.Message);
               }

               var flightDetailReadDto = _mapper.Map<FlightDetailReadDto>(flightDetailModel);

               return CreatedAtRoute(nameof(GetFligthDetailsByCode), new { flightCode = flightDetailReadDto.FlightCode}, flightDetailReadDto);
           }
           else
           {
               return NoContent();
           }
           
       }

        [HttpPut("{id}")]
       public ActionResult UpdateFlightDetail(int id, FlightDetailUpdateDto flightDetailUpdateDto){

           var flight = _context.FlightDetails.FirstOrDefault(x => x.Id == id);

           if (flight == null)
           {               
               return NoContent();
           }

            decimal oldPrice = flight.Price;

            _mapper.Map(flightDetailUpdateDto, flight);

            try
            {
                _context.SaveChanges();
                if (oldPrice != flight.Price)
                {
                    Console.WriteLine("Price changed - Place Message on bus");

                    var message = new NotificationMessageDto{
                        WebhookType = "pricechange",
                        OldPrice = oldPrice,
                        NewPrice = flight.Price,
                        FlightCode = flight.FlightCode
                    };

                    _messageBus.SendMessage(message);
                }
                else
                {
                    Console.WriteLine("No price change");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
            return NoContent();
       }

    }
}