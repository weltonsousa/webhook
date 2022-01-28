using Microsoft.EntityFrameworkCore;
using AirlineWeb.Models;

namespace AirlineWeb.Data{
    public class AirlineDbContext : DbContext {

        public AirlineDbContext(DbContextOptions<AirlineDbContext> options) : base(options){
            
        }

        public DbSet<WebhookSubscription> WebhookSubscriptions {get; set;}
        public DbSet<FlightDetail> FlightDetails {get; set;}
    }    
}