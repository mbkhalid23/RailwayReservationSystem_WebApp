using RailwayReservationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayReservationSystem.DataAccess.Repository.IRepository
{
    public interface IPassengerRepository : IRepository<Passenger>
    {
        void Update(Passenger obj);
        void Save();
    }
}
