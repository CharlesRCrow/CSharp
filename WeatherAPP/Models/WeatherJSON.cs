using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherAPP.Views.Home;

namespace WeatherAPP.Models
{
    public class WeatherJSON
    {
        public Dictionary<string, string> location { get; set; }
        public List<Dictionary<string, string>> weatherList { get; set; }

        public List<Dictionary<string, string>> WeatherGet()
        {
            try
            {
                location = WeatherSearch.GetLoc("Houston, Texas").Result;
                weatherList = WeatherSearch.GetWeather(location).Result;  
                return weatherList;
            }
            catch
            {
                return weatherList;
            }
                    
        }

        
    }
}