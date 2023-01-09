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
    public class ScheduleRepository : Repository<Schedule>, IScheduleRepository
    {
        private readonly ApplicationDbContext _db;

        public ScheduleRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Schedule obj)
        {
            _db.Schedule.Update(obj);
        }
    }
}
