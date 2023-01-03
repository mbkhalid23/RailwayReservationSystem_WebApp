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
            Train = new TrainRepository(_db);
            Station = new StationRepository(_db);
            Schedule = new ScheduleRepository(_db);
        }
        public ITrainRepository Train { get; private set; }
        public IStationRepository Station { get; private set; }
        public IScheduleRepository Schedule { get; set; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
