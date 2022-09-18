using RailwayReservationSystem.DataAccess.Data;
using RailwayReservationSystem.DataAccess.Repository.IRepository;
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
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Passenger = new PassengerRepository(_db);
            Train = new TrainRepository(_db);
            Station = new StationRepository(_db);
        }
        public IPassengerRepository Passenger { get; private set; }
        public ITrainRepository Train { get; set; }
        public IStationRepository Station { get; set; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
