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
    public class TrainRepository : Repository<Train>, ITrainRepository
    {
        private ApplicationDbContext _db;
        public TrainRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Train obj)
        {
            _db.Update(obj);
        }
    }
}
