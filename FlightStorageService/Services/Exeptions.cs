using System;

namespace FlightStorageService.Exceptions
{
    public abstract class FlightException : Exception
    {
        protected FlightException(string message) : base(message) { }
        protected FlightException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class FlightNotFoundException : FlightException
    {
        public string FlightNumber { get; }

        public FlightNotFoundException(string flightNumber) 
            : base($"Flight with number '{flightNumber}' was not found.")
        {
            FlightNumber = flightNumber;
        }
    }

    public class InvalidDateRangeException : FlightException
    {
        public DateTime RequestedDate { get; }

        public InvalidDateRangeException(DateTime requestedDate) 
            : base($"Requested date '{requestedDate:yyyy-MM-dd}' is outside the allowed range of 7 days from today.")
        {
            RequestedDate = requestedDate;
        }
    }

    public class InvalidFlightNumberException : FlightException
    {
        public string InvalidFlightNumber { get; }

        public InvalidFlightNumberException(string flightNumber) 
            : base($"Flight number '{flightNumber}' has invalid format. Flight number should be 2-10 characters long and contain only letters, numbers, and hyphens.")
        {
            InvalidFlightNumber = flightNumber;
        }
    }

    public class InvalidCityNameException : FlightException
    {
        public string InvalidCityName { get; }

        public InvalidCityNameException(string cityName) 
            : base($"City name '{cityName}' is invalid. City name should be 2-100 characters long and contain only letters, spaces, and common symbols.")
        {
            InvalidCityName = cityName;
        }
    }

    public class DatabaseConnectionException : FlightException
    {
        public DatabaseConnectionException(string message, Exception innerException) 
            : base($"Database connection failed: {message}", innerException) { }
    }

    public class FlightDataException : FlightException
    {
        public FlightDataException(string message, Exception innerException) 
            : base($"Flight data operation failed: {message}", innerException) { }
    }
}