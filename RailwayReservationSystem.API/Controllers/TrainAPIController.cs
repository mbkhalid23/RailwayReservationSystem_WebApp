using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RailwayReservationSystem.DataAccess.Repository.IRepository;
using RailwayReservationSystem.Models;
using RailwayReservationSystem.Models.Dto;
using System.Diagnostics;

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
            return Ok(_unitOfWork.Train.GetAll().ToList());

        }

        [HttpGet("{id:int}")]
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

            return Ok(train);
        }
    }
}
