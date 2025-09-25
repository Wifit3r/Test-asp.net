using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using FlightStorageService.Exceptions;

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

        [HttpGet("{FlightNumber}")]
        public async Task<ActionResult<Flight>> GetByNumber([FromRoute]string FlightNumber)
        {
            try
            {
                var result = await _flightRepository.GetByNumberAsync(FlightNumber);
                
                if (result == null)
                {
                    throw new FlightNotFoundException(FlightNumber);
                }

                return Ok(result);
            }
            catch (FlightNotFoundException)
            {
                return NotFound($"Flight with number '{FlightNumber}' was not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetByDate([FromQuery] DateTime date)
        {
            try
            {
                // Валідація дати (7 днів від сьогодні)
                var today = DateTime.Today;
                if (date.Date < today.AddDays(-1) || date.Date > today.AddDays(7))
                {
                    throw new InvalidDateRangeException(date);
                }

                var results = await _flightRepository.GetByDateAsync(date);
                
                // Повертаємо Ok навіть якщо список порожній
                return Ok(results);
            }
            catch (InvalidDateRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("departure")]
        public async Task<ActionResult<IEnumerable<Flight>>> GetByDepartDateAndCity([FromQuery] DateTime date, [FromQuery] string departure)
        {
            try
            {
                // Валідація дати
                var today = DateTime.Today;
                if (date.Date < today.AddDays(-1) || date.Date > today.AddDays(7))
                {
                    throw new InvalidDateRangeException(date);
                }

                // Валідація назви міста
                if (string.IsNullOrWhiteSpace(departure) || departure.Length < 2 || departure.Length > 100)
                {
                    throw new InvalidCityNameException(departure ?? "null");
                }

                var results = await _flightRepository.GetByDepartureCityAndDateAsync(departure, date);
                
                return Ok(results);
            }
            catch (InvalidDateRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidCityNameException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("arrival")]
        public async Task<ActionResult<IEnumerable<Flight>>> GetByArrivalDateAndCity([FromQuery] DateTime date, [FromQuery] string arrival)
        {
            try
            {
                // Валідація дати
                var today = DateTime.Today;
                if (date.Date < today.AddDays(-1) || date.Date > today.AddDays(7))
                {
                    throw new InvalidDateRangeException(date);
                }

                // Валідація назви міста
                if (string.IsNullOrWhiteSpace(arrival) || arrival.Length < 2 || arrival.Length > 100)
                {
                    throw new InvalidCityNameException(arrival ?? "null");
                }

                var results = await _flightRepository.GetByArrivalCityAndDateAsync(arrival, date);
                
                return Ok(results);
            }
            catch (InvalidDateRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidCityNameException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Flight>> PostNewFlight([FromBody] Flight newFlight)
        {
            if (newFlight == null) 
                return BadRequest("Flight data is required");

            try
            {
                // Валідація номера рейсу
                if (string.IsNullOrWhiteSpace(newFlight.FlightNumber) || 
                    newFlight.FlightNumber.Length < 2 || 
                    newFlight.FlightNumber.Length > 10)
                {
                    throw new InvalidFlightNumberException(newFlight.FlightNumber ?? "null");
                }

                // Валідація міст
                if (string.IsNullOrWhiteSpace(newFlight.DepartureAirportCity) || 
                    newFlight.DepartureAirportCity.Length < 2 || 
                    newFlight.DepartureAirportCity.Length > 100)
                {
                    throw new InvalidCityNameException(newFlight.DepartureAirportCity ?? "null");
                }

                if (string.IsNullOrWhiteSpace(newFlight.ArrivalAirportCity) || 
                    newFlight.ArrivalAirportCity.Length < 2 || 
                    newFlight.ArrivalAirportCity.Length > 100)
                {
                    throw new InvalidCityNameException(newFlight.ArrivalAirportCity ?? "null");
                }

                // Валідація дати
                var today = DateTime.Today;
                if (newFlight.DepartureDateTime.Date < today || newFlight.DepartureDateTime.Date > today.AddDays(7))
                {
                    throw new InvalidDateRangeException(newFlight.DepartureDateTime);
                }

                // Валідація тривалості
                if (newFlight.DurationMinutes <= 0)
                {
                    return BadRequest("Flight duration must be greater than 0 minutes");
                }

                if (newFlight.DurationMinutes > 24 * 60)
                {
                    return BadRequest("Flight duration cannot exceed 24 hours");
                }

                await _flightRepository.AddFlightAsync(newFlight);
                
                return CreatedAtAction(nameof(GetByNumber), 
                    new { FlightNumber = newFlight.FlightNumber }, 
                    newFlight);
            }
            catch (InvalidFlightNumberException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidCityNameException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidDateRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Npgsql.PostgresException ex)
            {
                return BadRequest($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}