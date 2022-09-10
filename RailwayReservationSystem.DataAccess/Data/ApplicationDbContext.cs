using Microsoft.EntityFrameworkCore;
using RailwayReservationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayReservationSystem.DataAccess.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        DbSet<Passenger> Passengers { get; set; }
        DbSet<Reservation> Reservations { get; set; }
        DbSet<Train> Trains { get; set; }
        DbSet<Station> Stations { get; set; }
        DbSet<Schedule> Schedule { get; set; }

    }
}
