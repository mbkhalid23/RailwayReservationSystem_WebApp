using RailwayReservationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayReservationSystem.DataAccess.Repository.IRepository
{
    public interface ITrainRepository : IRepository<Train>
    {
        void Update(Train obj);
        Train GetLast(string? IncludeProperties = null, bool tracked = true);
    }
}
