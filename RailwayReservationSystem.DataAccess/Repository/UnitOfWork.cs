using Microsoft.Extensions.Configuration;
using RailwayReservationSystem.DataAccess.Data;
using RailwayReservationSystem.DataAccess.Repository.IRepository;
using RailwayReservationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayReservationSystem.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        public UnitOfWork(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
            Train = new TrainRepository(_db);
            Station = new StationRepository(_db);
            Schedule = new ScheduleRepository(_db);
            BookingCart = new BookingCartRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
			OrderHeader = new OrderHeaderRepository(_db);
			OrderDetail = new OrderDetailRepository(_db);
            LocalUser = new UserRepository(_db, _configuration);
		}
        public ITrainRepository Train { get; private set; }
        public IStationRepository Station { get; private set; }
        public IScheduleRepository Schedule { get; set; }
        public IBookingCartRepository BookingCart { get; set; }
        public IApplicationUserRepository ApplicationUser { get; set; }
		public IOrderHeaderRepository OrderHeader { get; set; }
		public IOrderDetailRepository OrderDetail { get; set; }
        public IUserRepository LocalUser { get; set; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
