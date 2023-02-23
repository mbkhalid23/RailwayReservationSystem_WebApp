using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RailwayReservationSystem.DataAccess.Repository.IRepository;
using RailwayReservationSystem.Models;

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
        public ActionResult<IEnumerable<Train>> GetAll()
        {
            IEnumerable<Train> trains = _unitOfWork.Train.GetAll().ToList();
            return Ok(trains);
        }
    }
}
