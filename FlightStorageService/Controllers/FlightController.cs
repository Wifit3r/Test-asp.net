using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace FlightStorageService
{
    [ApiController]
    [Route("api/flights")]
    public class FlightController : ControllerBase
    {
        private readonly FlightRepository _flightRepository;

        public FlightController(FlightRepository flightRepository)
        {
            _flightRepository = flightRepository;
        }

        [HttpGet("{flightNumber}")]
        public async Task<ActionResult<Flight>> GetByNumber(string flightNumber)
        {
            var result = await _flightRepository.GetByNumberAsync(flightNumber);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetByDate([FromQuery] DateTime date)
        {
            var results = await _flightRepository.GetByDateAsync(date);
            if (!results.Any()) return NotFound();
            return Ok(results);
        }

        [HttpGet("departure")]
        public async Task<ActionResult<IEnumerable<Flight>>> GetByDepartDateAndCity([FromQuery] DateTime date, [FromQuery] string departure)
        {
            var results = await _flightRepository.GetByDepartureCityAndDateAsync(departure, date);
            if (!results.Any()) return NotFound();
            return Ok(results);
        }

        [HttpGet("arrival")]
        public async Task<ActionResult<IEnumerable<Flight>>> GetByArrivalDateAndCity([FromQuery] DateTime date, [FromQuery] string arrival)
        {
            var results = await _flightRepository.GetByArrivalCityAndDateAsync(arrival, date);
            if (!results.Any()) return NotFound();
            return Ok(results);
        }

        [HttpPost]
        public async Task<ActionResult<Flight>> PostNewFlight([FromBody] Flight newFlight)
        {
            if (newFlight == null) return BadRequest();

            try
            {
                await _flightRepository.AddFlightAsync(newFlight);
            }
            catch (Npgsql.PostgresException ex)
            {
                return BadRequest(ex.Message);
            }

            return CreatedAtAction(nameof(GetByNumber), new { flightNumber = newFlight.FlightNumber }, newFlight);
        }
    }
}