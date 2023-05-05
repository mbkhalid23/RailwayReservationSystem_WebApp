using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RailwayReservationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayReservationSystem.DataAccess.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<Train> Trains { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Schedule> Schedule { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<BookingCart> BookingCarts { get; set; }
		public DbSet<OrderHeader> OrderHeaders { get; set; }
		public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<LocalUser> LocalUsers { get; set; }
    }
}
