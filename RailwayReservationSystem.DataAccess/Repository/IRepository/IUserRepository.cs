using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RailwayReservationSystem.Models;
using RailwayReservationSystem.Models.Dto;

namespace RailwayReservationSystem.DataAccess.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUserName(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<LocalUser> Register(RegistrationRequestDTO registrationRequestDTO);
        
    }
}
