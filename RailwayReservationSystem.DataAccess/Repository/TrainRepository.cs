using Microsoft.EntityFrameworkCore;
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
        private readonly ApplicationDbContext _db;
        public TrainRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Train obj)
        {
            _db.Trains.Update(obj);
        }
        public Train GetLast(string? IncludeProperties = null, bool tracked = true)
        {
            IQueryable<Train> query;

            if (tracked)
            {
                query = dbSet;
            }
            else
            {
                query = dbSet.AsNoTracking();
            }

            if (IncludeProperties != null)
            {
                foreach (var includeProp in IncludeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            query = query.OrderByDescending(t=>t.TrainNo);

            return query.FirstOrDefault();
        }
    }
}
