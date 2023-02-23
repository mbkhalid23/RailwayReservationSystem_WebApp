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

        public TrainAPIController(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<TrainDTO>> GetAll()
        {
            IEnumerable<Train> trains = _unitOfWork.Train.GetAll().ToList();
            List<TrainDTO> trainDTOs = new();

            foreach (var train in trains)
            {
                //Copy Train model to TrainDTO 
                TrainDTO trainDTO = new()
                {
                    TrainNo = train.TrainNo,
                    Name = train.Name,
                    Capacity = train.Capacity,
                    Status = train.Status,
                    StationId = train.StationId
                };

                trainDTOs.Add(trainDTO);
            }

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
            TrainDTO trainDTO = new()
            {
                TrainNo = train.TrainNo,
                Name = train.Name,
                Capacity = train.Capacity,
                Status = train.Status,
                StationId = train.StationId
            };

            return Ok(trainDTO);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<TrainCreateDTO> AddNewTrain([FromBody] TrainCreateDTO trainDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (trainDTO.Capacity < 50)
            {
                ModelState.AddModelError("", "train cannot have less than 50 seats");

                return BadRequest(ModelState);
            }

            //Copy TrainDTO to Train model
            Train train = new()
            {
                Name = trainDTO.Name,
                Capacity = trainDTO.Capacity,
                Status = trainDTO.Status,
                StationId = trainDTO.StationId
            };

            //Get station
            train.Station = _unitOfWork.Station.GetFirstOrDefault(s => s.StationId == train.StationId);

            if (train.Station == null)
            {
                ModelState.AddModelError("", "No Station found with the given id");
                return NotFound(ModelState);
            }

            //Check if the station has space available  for the train
            if (train.Station.AvailableSlots <= 0)
            {
                ModelState.AddModelError("", "Station already at Max capacity. Can't Add a new Train");
                return BadRequest(ModelState);
            }

            //Train arrives at the station
            train.Station.TrainsStationed++;
            train.Station.AvailableSlots--;

            //Add the train to database
            _unitOfWork.Train.Add(train);
            _unitOfWork.Save();

            //Copy train to trainDTO
            trainDTO.Name = train.Name;
            trainDTO.Capacity = train.Capacity;
            trainDTO.Status = train.Status;
            trainDTO.StationId = train.StationId;

            return CreatedAtRoute("GetById",new { id = train.TrainNo}, trainDTO);
        }
    }
}
