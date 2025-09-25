using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FlightStorageService.Pages
{
    public class FlightsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public FlightsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime? Date { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ArrivalCity { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string DepartureCity { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string? FlightNumber { get; set; } = string.Empty;

        public List<Flight> Flights { get; set; } = new List<Flight>();

        public async Task OnGetAsync()
        {
            Flights = new List<Flight>();
            
            // Якщо є номер рейсу - шукаємо за номером
            if (!string.IsNullOrWhiteSpace(FlightNumber))
            {
                await SearchByFlightNumber();
                return;
            }
            
            // Інакше шукаємо за параметрами
            if (!Date.HasValue)
                return;

            await SearchByParameters();
        }

        private async Task SearchByFlightNumber()
        {
            var client = _httpClientFactory.CreateClient();
            string url = $"http://localhost:5228/api/flights/{Uri.EscapeDataString(FlightNumber)}";

            try
            {
                var response = await client.GetFromJsonAsync<Flight>(url);
                if (response != null)
                    Flights = new List<Flight> { response };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Flights = new List<Flight>();
            }
        }

        private async Task SearchByParameters()
        {
            string dateString = Date.Value.ToString("yyyy-MM-dd");
            var client = _httpClientFactory.CreateClient();
            string url;

            if (!string.IsNullOrWhiteSpace(ArrivalCity))
                url = $"http://localhost:5228/api/flights/arrival?date={dateString}&arrival={Uri.EscapeDataString(ArrivalCity)}";
            else if (!string.IsNullOrWhiteSpace(DepartureCity))
                url = $"http://localhost:5228/api/flights/departure?date={dateString}&departure={Uri.EscapeDataString(DepartureCity)}";
            else
                url = $"http://localhost:5228/api/flights?date={dateString}";

            try
            {
                Flights = await client.GetFromJsonAsync<List<Flight>>(url) ?? new List<Flight>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Flights = new List<Flight>();
            }
        }
    }

    public class Flight
    {
        public string FlightNumber { get; set; } = string.Empty;
        public DateTime DepartureDateTime { get; set; }
        public string DepartureAirportCity { get; set; } = string.Empty;
        public string ArrivalAirportCity { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
    }
}