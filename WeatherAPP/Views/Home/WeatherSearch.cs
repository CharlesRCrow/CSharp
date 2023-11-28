using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace WeatherAPP.Views.Home
{
 public class WeatherSearch
    {
        public static async Task<Dictionary<string, string>> GetLoc(string address)
        {
            HttpClient locationClient = new HttpClient();
            HttpRequestMessage locationRequest = new HttpRequestMessage(HttpMethod.Get, $"https://geocode.maps.co/search?q={address}");

            HttpResponseMessage locationHttpResponseMessage = await locationClient.SendAsync(locationRequest);
            string locationResponse = await locationHttpResponseMessage.Content.ReadAsStringAsync();

            //Dictionary<string, string> invalid = new Dictionary<string, string>();
            
            if (locationResponse == "[]")
            {
                return new Dictionary<string, string>();
            }

            string firstResponse = locationResponse.Split('{', '}')[1];
            firstResponse = "{" + firstResponse + "}";

            JObject jsonLocation = JObject.Parse(firstResponse);
            JToken? latitude = jsonLocation.SelectToken("lat");
            JToken? longitude = jsonLocation.SelectToken("lon");

            if (latitude?.Type != JTokenType.Null && longitude?.Type != JTokenType.Null)
            {
                Dictionary<string, string> latLong = new Dictionary<string, string>
                {
                    { "Latitude", (string) latitude!},
                    { "Longitude", (string) longitude!}
                };

                return latLong;   
            }
            else
            {
                return new Dictionary<string, string>();
            }
        }

        public static async Task<List<Dictionary<string, string>>> GetWeather(Dictionary<string, string> latLong, string weatherSelect)
        {
            List<Dictionary<string, string>> invalid = new List<Dictionary<string, string>>();

            string latitude = latLong["Latitude"];
            string longitude = latLong["Longitude"];

            string weatherLocation = $"https://api.weather.gov/points/{latitude},{longitude}";
            
            // get grid id, x and y for forecast                               
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, weatherLocation);

            ProductInfoHeaderValue header = new ProductInfoHeaderValue("WeatherCrow", "1.0");
            ProductInfoHeaderValue comment = new ProductInfoHeaderValue("(+http://www.crowweather.com/WeatherCrow.html)");

            client.DefaultRequestHeaders.UserAgent.Add(header);
            client.DefaultRequestHeaders.UserAgent.Add(comment);

            HttpResponseMessage httpResponseMessage = await client.SendAsync(request);
            string response = await httpResponseMessage.Content.ReadAsStringAsync();

            JObject root = JObject.Parse(response);
            JToken? token = root.SelectToken("properties");
            
            if (token is null)
            {
                return new List<Dictionary<string, string>>();
            }
            
            JToken? xCord = token.SelectToken("gridX");
            JToken? yCord = token.SelectToken("gridY");
            JToken? gridID = token.SelectToken("gridId");

            HttpRequestMessage? forecastRequest = null;
            
            if (weatherSelect.Equals("seven"))
            {
                forecastRequest = new HttpRequestMessage(HttpMethod.Get, 
                    $"https://api.weather.gov/gridpoints/{gridID}/{xCord},{yCord}/forecast");
            }
            else if (weatherSelect.Equals("hourly"))
            {
                forecastRequest = new HttpRequestMessage(HttpMethod.Get, 
                    $"https://api.weather.gov/gridpoints/{gridID}/{xCord},{yCord}/forecast/hourly");
            }
            else
            {
                return new List<Dictionary<string, string>>();
            }

            HttpResponseMessage httpResponseForecast = await client.SendAsync(forecastRequest);

            string forecastResponse = await httpResponseForecast.Content.ReadAsStringAsync();

            JObject forecast = (JObject)JObject.Parse(forecastResponse)["properties"]!;
            JArray dailyForecast = (JArray)forecast["periods"]!;
            
            if (dailyForecast is null)
            {
                return new List<Dictionary<string, string>>();
            }
            
            List<Dictionary<string, string>> weatherList = new List<Dictionary<string, string>>();            

            foreach (JToken day in dailyForecast ?? throw new InvalidOperationException())
            {
                Dictionary<string, string> dayWeather = new();
                
                if (day.Type != JTokenType.Null)
                {
                    dayWeather = new()
                    {
                        { "Name", (string)day["name"]! },
                        { "Temp", (string)day["temperature"]! },
                        { "WindSpeed", (string)day["windSpeed"]! },
                        { "WindDirection", (string)day["windDirection"]! },
                        { "Humidity", (string)day["relativeHumidity"]!["value"]! },
                        { "Dewpoint", (string)day["dewpoint"]!["value"]! },
                        { "DailyPrecipitation", (string)day["probabilityOfPrecipitation"]!["value"]! },
                        { "DetailedForecast", (string)day["detailedForecast"]! },
                        { "ShortForecast", (string)day["shortForecast"]! },
                        { "StartTime", (string)day["startTime"]! },
                        { "EndTime", (string)day["endTime"]! }
                    };

                    if (dayWeather[$"DailyPrecipitation"] is null)
                    {
                        dayWeather[$"DailyPrecipitation"] = "0";
                    }
                    
                    DateTime startTime = DateTime.Parse(dayWeather["StartTime"]);
                    DateTime endTime = DateTime.Parse(dayWeather["EndTime"]);

                    dayWeather["Period"] = $"{startTime.DayOfWeek}  {startTime.Hour}:00 to {endTime.Hour}:00";
                }
                weatherList.Add(dayWeather);
            }

            if (weatherList.Count == 0)
            {
                return new List<Dictionary<string, string>>();
            }
            else
            {
                return weatherList;
            }
        }
    }
}