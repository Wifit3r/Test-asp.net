using Npgsql;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace FlightStorageService
{
    public class FlightRepository
    {
        private readonly string _connectionString;

        public FlightRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Flight?> GetByNumberAsync(string flightNumber)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            // Додаємо фільтр за датою безпосередньо у запит
            var query = @"
                SELECT * 
                FROM get_flight_by_number(@_flight_number)
                WHERE departure_date_time BETWEEN NOW()::date - INTERVAL '7 days' AND NOW()::date + INTERVAL '7 days';
            ";

            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("_flight_number", NpgsqlTypes.NpgsqlDbType.Varchar, flightNumber);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return ReadFlightFromReader(reader);
            }
            return null;
        }
        public async Task<IEnumerable<Flight>> GetByDateAsync(System.DateTime date)
        {
            var flights = new List<Flight>();
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT * FROM get_flights_by_date(@_date) WHERE departure_date_time BETWEEN NOW()::date - INTERVAL '7 days' AND NOW()::date + INTERVAL '7 days' ";
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("_date", NpgsqlTypes.NpgsqlDbType.Date, date.Date);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                flights.Add(ReadFlightFromReader(reader));
            }
            return flights;
        }

        public async Task<IEnumerable<Flight>> GetByDepartureCityAndDateAsync(string city, System.DateTime date)
        {
            var flights = new List<Flight>();
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT * FROM get_flights_by_departure_city_and_date(@_city, @_date)  WHERE departure_date_time BETWEEN NOW()::date - INTERVAL '7 days' AND NOW()::date + INTERVAL '7 days';";
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("_city", NpgsqlTypes.NpgsqlDbType.Varchar, city);
            cmd.Parameters.AddWithValue("_date", NpgsqlTypes.NpgsqlDbType.Date, date.Date);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                flights.Add(ReadFlightFromReader(reader));
            }
            return flights;
        }

        public async Task<IEnumerable<Flight>> GetByArrivalCityAndDateAsync(string city, System.DateTime date)
        {
            var flights = new List<Flight>();
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT * FROM get_flights_by_arrival_city_and_date(@_city, @_date)  WHERE departure_date_time BETWEEN NOW()::date - INTERVAL '7 days' AND NOW()::date + INTERVAL '7 days';";
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("_city", NpgsqlTypes.NpgsqlDbType.Varchar, city);
            cmd.Parameters.AddWithValue("_date", NpgsqlTypes.NpgsqlDbType.Date, date.Date);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                flights.Add(ReadFlightFromReader(reader));
            }
            return flights;
        }

        public async Task AddFlightAsync(Flight newFlight)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = @"
                SELECT add_flight(
                    @_flight_number,
                    @_departure_date_time,
                    @_departure_airport_city,
                    @_arrival_airport_city,
                    @_duration_minutes
                );";
            await using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("_flight_number", NpgsqlTypes.NpgsqlDbType.Varchar, newFlight.FlightNumber);
            cmd.Parameters.AddWithValue("_departure_date_time", NpgsqlTypes.NpgsqlDbType.TimestampTz, newFlight.DepartureDateTime);
            cmd.Parameters.AddWithValue("_departure_airport_city", NpgsqlTypes.NpgsqlDbType.Varchar, newFlight.DepartureAirportCity);
            cmd.Parameters.AddWithValue("_arrival_airport_city", NpgsqlTypes.NpgsqlDbType.Varchar, newFlight.ArrivalAirportCity);
            cmd.Parameters.AddWithValue("_duration_minutes", NpgsqlTypes.NpgsqlDbType.Integer, newFlight.DurationMinutes);

            await cmd.ExecuteNonQueryAsync();
        }

        private Flight ReadFlightFromReader(NpgsqlDataReader reader)
        {
            return new Flight
            {
                FlightNumber = reader.GetString("flight_number"),
                DepartureDateTime = reader.GetDateTime("departure_date_time"),
                DepartureAirportCity = reader.GetString("departure_airport_city"),
                ArrivalAirportCity = reader.GetString("arrival_airport_city"),
                DurationMinutes = reader.GetInt32("duration_minutes")
            };
        }
    }
}