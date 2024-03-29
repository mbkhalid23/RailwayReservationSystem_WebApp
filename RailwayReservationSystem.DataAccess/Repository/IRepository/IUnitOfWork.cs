﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayReservationSystem.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ITrainRepository Train { get; }
        IStationRepository Station { get; }
        IScheduleRepository Schedule { get; }
        IBookingCartRepository BookingCart { get; }
        IApplicationUserRepository ApplicationUser { get; }
		IOrderHeaderRepository OrderHeader { get; }
		IOrderDetailRepository OrderDetail { get; }
        IUserRepository LocalUser { get; }
        void Save();
    }
}
