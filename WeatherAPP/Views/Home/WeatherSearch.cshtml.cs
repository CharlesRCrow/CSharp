using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WeatherAPP.Views.Home
{
    public class WeatherSearchModel : PageModel
    {
        public Dictionary<string, string> location { get; set; }
        public List<Dictionary<string, string>> weatherList { get; set; }

        public void OnGet()
        {
            location = WeatherSearch.GetLoc("Houston, Texas").Result;
            weatherList = WeatherSearch.GetWeather(location).Result;
        }

        public void Search(string searchInput)
        {
            location = WeatherSearch.GetLoc(searchInput).Result;
            weatherList = WeatherSearch.GetWeather(location).Result;
        }
    }
}