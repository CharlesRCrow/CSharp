using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WeatherWeb;

namespace WeatherWeb.Views.Home
{
    public class Index : PageModel
    {
        public List<Dictionary<string, string>>? weatherList { get; set; }
        public Dictionary<string, string>? address { get; set; }

        private readonly ILogger<Index> _logger;

        public Index(ILogger<Index> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            address = WeatherSearch.GetLoc("Angleton, Texas").Result;
            //weatherList = WeatherSearch.GetWeather(address).Result;
            weatherList = null;
        }
    }
}