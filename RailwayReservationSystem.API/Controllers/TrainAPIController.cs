using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RailwayReservationSystem.DataAccess.Repository.IRepository;
using RailwayReservationSystem.Models;
using RailwayReservationSystem.Models.Dto;
using System.Diagnostics;
using static RailwayReservationSystem.Models.Train;

namespace RailwayReservationSystem.API.Controllers
{
    [Route("api/TrainAPI")]
    [ApiController]
    public class TrainAPIController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TrainAPIController(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<TrainDTO>> GetAll()
        {
            IEnumerable<Train> trains = _unitOfWork.Train.GetAll().ToList();

            //Copy trains list to TrainDTOs list
            List<TrainDTO> trainDTOs = _mapper.Map<List<TrainDTO>>(trains);

            return Ok(trainDTOs);
        }

        [HttpGet("{id:int}", Name = "GetById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TrainDTO> GetById(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var train = _unitOfWork.Train.GetFirstOrDefault(t=>t.TrainNo== id);
            if (train == null)
            {
                return NotFound();
            }

            //Copy Train model to TrainDTO 
            TrainDTO trainDTO = _mapper.Map<TrainDTO>(train);

            return Ok(trainDTO);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<TrainCreateDTO> AddNewTrain([FromBody] TrainCreateDTO trainCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (trainCreateDTO.Capacity < 50)
            {
                ModelState.AddModelError("CustomError", "train cannot have less than 50 seats");

                return BadRequest(ModelState);
            }

            //Copy TrainDTO to Train model
            Train train = _mapper.Map<Train>(trainCreateDTO);

            //Get station
            train.Station = _unitOfWork.Station.GetFirstOrDefault(s => s.StationId == train.StationId);

            if (train.Station == null)
            {
                ModelState.AddModelError("CustomError", "No Station found with the given id");
                return NotFound(ModelState);
            }

            //Check if the station has space available  for the train
            if (train.Station.AvailableSlots <= 0)
            {
                ModelState.AddModelError("CustomError", "Station already at Max capacity. Can't Add a new Train");
                return BadRequest(ModelState);
            }

            //Train arrives at the station
            train.Station.TrainsStationed++;
            train.Station.AvailableSlots--;

            //Add the train to database
            _unitOfWork.Train.Add(train);
            _unitOfWork.Save();

            //Copy train to trainDTO
            trainCreateDTO = _mapper.Map<TrainCreateDTO>(train);

            return CreatedAtRoute("GetById",new { id = train.TrainNo}, trainCreateDTO);
        }
    }
}
