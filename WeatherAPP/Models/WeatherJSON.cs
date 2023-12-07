using WeatherAPP.Views.Models;

namespace WeatherAPP.Models
{
    public class WeatherJSON
    {
        public  Dictionary<string, string>? location { get; set; }
        public List<Dictionary<string, string>>? weatherList { get; set; }
        
        public class SearchViewModel
        {
            public string? Query { get; set; }
        }

        public List<Dictionary<string, string>> WeatherGet(string address, string weatherSelect)
        {
            try
            {
                location = WeatherSearch.GetLoc(address).Result;
                weatherList = WeatherSearch.GetWeather(location, weatherSelect).Result;  
                return weatherList;
            }
            catch
            {
                return new List<Dictionary<string, string>>();
            }       
        }
        public List<Dictionary<string, string>> WeatherGet()
        {
            return new List<Dictionary<string, string>>();                    
        }        
    }
}