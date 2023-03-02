using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RailwayReservationSystem.DataAccess.Repository.IRepository;
using RailwayReservationSystem.Models;
using RailwayReservationSystem.Models.Dto;
using System.Collections.Generic;
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

        [Authorize]
        [HttpGet]
        [ResponseCache(CacheProfileName = "Default")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<IEnumerable<TrainDTO>> GetAll([FromQuery] int? capacity, [FromQuery] int? station, int pageSize = 3, int pageNumber = 1)
        {
            IEnumerable<Train> trains;
            if (capacity > 0)
            {
                trains = _unitOfWork.Train.GetAll(t => t.Capacity >= capacity, pageSize:pageSize, pageNumber:pageNumber);
            }
            else
            {
                trains = _unitOfWork.Train.GetAll(pageSize: pageSize, pageNumber: pageNumber).ToList();

            }

            if (station > 0)
            {
                trains = trains.Where(t => t.StationId ==  station);
            }

            //Copy trains list to TrainDTOs list
            List<TrainDTO> trainDTOs = _mapper.Map<List<TrainDTO>>(trains);

            return Ok(trainDTOs);
        }

        [Authorize]
        [HttpGet("{id:int}", Name = "GetById")]
        [ResponseCache(CacheProfileName = "Default")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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

        [Authorize(Roles ="admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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

        [Authorize(Roles = "admin")]
        [HttpDelete("{id:int}", Name = "DeleteTrain")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult DeleteTrain(int id)
        {
            //Check if the id is null or 0
            if (id == null || id == 0)
            {
                return BadRequest();
            }

            //Get train along with its station
            Train train = _unitOfWork.Train.GetFirstOrDefault(t => t.TrainNo == id, IncludeProperties: "Station");

            //Check if the train object is null
            if (train == null)
            {
                return NotFound();
            }

            //Train leaves the station
            train.Station.TrainsStationed--;
            train.Station.AvailableSlots++;

            //Remove the train from database
            _unitOfWork.Train.Remove(train);
            _unitOfWork.Save();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}", Name = "UpdateTrain")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateTrain(int id, [FromBody] TrainUpdateDTO trainUpdateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (trainUpdateDTO == null || id != trainUpdateDTO.TrainNo)
            {
                return BadRequest();
            }

            if (trainUpdateDTO.Capacity < 50)
            {
                ModelState.AddModelError("CustomError", "train cannot have less than 50 seats");

                return BadRequest(ModelState);
            }

            Train train = _unitOfWork.Train.GetFirstOrDefault(t => t.TrainNo == id);
            //store current station id
            trainUpdateDTO.PrevStation = train.StationId;

            _mapper.Map(trainUpdateDTO,train);

            if (train.StationId != trainUpdateDTO.PrevStation)
            {
                

                //Get the new station of Train
                train.Station = _unitOfWork.Station.GetFirstOrDefault(s => s.StationId == train.StationId);

                if (train.Station.AvailableSlots <= 0)
                {
                    ModelState.AddModelError("constraint", "No space available on the new station");
                    return BadRequest();
                }

                //Train arrives at the new station
                train.Station.TrainsStationed++;
                train.Station.AvailableSlots--;

                //Get the previous station of Train
                Station oldStation = _unitOfWork.Station.GetFirstOrDefault(s=>s.StationId== trainUpdateDTO.PrevStation);

                //Train leave the previous station
                oldStation.TrainsStationed--;
                oldStation.AvailableSlots++;

                _unitOfWork.Station.Update(oldStation);
            }

            _unitOfWork.Train.Update(train);
            _unitOfWork.Save();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("{id:int}", Name = "UpdatePartialTrain")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult UpdatePartialTrain(int id, JsonPatchDocument<TrainUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            
            var train = _unitOfWork.Train.GetFirstOrDefault(t => t.TrainNo == id);
           
            if (train == null)
            {
                return NotFound();
            }

            var updateDTO = _mapper.Map<TrainUpdateDTO>(train);

            //store current station id
            updateDTO.PrevStation = train.StationId;

            patchDTO.ApplyTo(updateDTO);

            _mapper.Map(updateDTO, train);

            if (train.StationId != updateDTO.PrevStation)
            {
                //Get the new station of Train
                train.Station = _unitOfWork.Station.GetFirstOrDefault(s => s.StationId == train.StationId);

                if (train.Station.AvailableSlots <= 0)
                {
                    ModelState.AddModelError("constraint", "No space available on the new station");
                    return BadRequest();
                }

                //Train arrives at the new station
                train.Station.TrainsStationed++;
                train.Station.AvailableSlots--;

                //Get the previous station of Train
                Station oldStation = _unitOfWork.Station.GetFirstOrDefault(s => s.StationId == updateDTO.PrevStation);

                //Train leave the previous station
                oldStation.TrainsStationed--;
                oldStation.AvailableSlots++;

                _unitOfWork.Station.Update(oldStation);
            }

            _unitOfWork.Train.Update(train);
            _unitOfWork.Save();

            return Ok(updateDTO);
        }
    }
}
