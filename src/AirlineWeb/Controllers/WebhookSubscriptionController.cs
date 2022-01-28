using System;
using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AirlineWeb.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Linq;

namespace AirlineWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookSubscriptionController : ControllerBase
    {       
        private readonly AirlineDbContext _context;
        private readonly IMapper _mapper;
        public WebhookSubscriptionController(AirlineDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{secret}", Name = "GetSbuscriptionBySecret")]
        public ActionResult<WebhookSubscriptionReadDto> GetSbuscriptionBySecret(string secret){

            var subscription = _context.WebhookSubscriptions.FirstOrDefault(s => s.Secret == secret);

            if (subscription == null){
                return NotFound();
            }

            return Ok(_mapper.Map<WebhookSubscriptionReadDto>(subscription));
        }

        [HttpPost]        
        public ActionResult<WebhookSubscriptionReadDto> CreateSubscription(WebhookSubscriptionCreateDto webhookSubscriptionCreateDto)
        {
            
            var subscription = _context.WebhookSubscriptions.FirstOrDefault(x => x.WebhookURI == webhookSubscriptionCreateDto.WebhookURI);

            if (subscription == null)
            {
                subscription = _mapper.Map<WebhookSubscription>(webhookSubscriptionCreateDto);
                subscription.Secret = Guid.NewGuid().ToString();
                subscription.WebhookPublisher = "PanAus";
                try{
                    _context.WebhookSubscriptions.Add(subscription);
                    _context.SaveChanges();
                }
                catch(Exception ex){
                    return BadRequest(ex.Message);
                }

                var WebhookSubscriptionReadDto = _mapper.Map<WebhookSubscriptionReadDto>(subscription);

                return CreatedAtRoute(nameof(GetSbuscriptionBySecret), new { secret = WebhookSubscriptionReadDto.Secret}, WebhookSubscriptionReadDto);
            }
            else
            {
                return NoContent();
            }
        }
       
    }
}