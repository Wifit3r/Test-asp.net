using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlightStorageService.Pages
{
    public class FlightsModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public FlightsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [BindProperty]
        public string Date { get; set; }

        [BindProperty]
        public string ArrivalCity { get; set; } 

        public string DepCity { get; set; }

        public List<Flight> Flights { get; set; } = new List<Flight>();

        public async Task OnPostAsync()
        {
            if (string.IsNullOrEmpty(Date)) return;

            // Створюємо URL запиту
            var url = $"http://localhost:5228/api/flights?date={Date}";
            if (!string.IsNullOrEmpty(ArrivalCity))
            {
                url = $"http://localhost:5228/api/flights/arrival?date={Date}&city={ArrivalCity}";
            }
            if (!string.IsNullOrEmpty(DepCity))
            {
                url = $"http://localhost:5228/api/flights/departure?date={Date}&city={DepCity}";
            }

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                Flights = await response.Content.ReadFromJsonAsync<List<Flight>>();
            }
        }
    }

    public class Flight
    {
        public string FlightNumber { get; set; }
        public System.DateTime DepartureDateTime { get; set; }
        public string DepartureAirportCity { get; set; }
        public string ArrivalAirportCity { get; set; }
        public int DurationMinutes { get; set; }
    }
}