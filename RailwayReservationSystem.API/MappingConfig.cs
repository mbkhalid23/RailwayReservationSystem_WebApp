using AutoMapper;
using RailwayReservationSystem.Models;
using RailwayReservationSystem.Models.Dto;

namespace RailwayReservationSystem.API
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Train,TrainDTO>().ReverseMap();
            CreateMap<Train, TrainCreateDTO>().ReverseMap();
            CreateMap<Train, TrainUpdateDTO>().ReverseMap();
            CreateMap<List<TrainDTO>,IEnumerable<Train>>();


        }
    }
}
