using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayReservationSystem.Models.Dto
{
    internal class LoginResponseDTO
    {
        public LocalUser User { get; set; }
        public string Token { get; set; }

    }
}
