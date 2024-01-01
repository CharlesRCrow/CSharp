using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace WeatherAPP.Views.Models
{
 public class WeatherSearch
    {
        public static async Task<Dictionary<string, string>> GetLoc(string address)
        {
            HttpClient locationClient = new HttpClient();
            HttpRequestMessage locationRequest = new HttpRequestMessage(HttpMethod.Get, $"https://geocode.maps.co/search?q={address}&api_key=658c6db1d8ea5951371902vxcf86d7c");

            HttpResponseMessage locationHttpResponseMessage = await locationClient.SendAsync(locationRequest);
            
            if (!locationHttpResponseMessage.IsSuccessStatusCode)
            {
                Dictionary<string, string> error = new Dictionary<string, string>
                {
                    { "Error", "Connection Unsuccessful"}
                };

                return error;
            }

            string locationResponse = await locationHttpResponseMessage.Content.ReadAsStringAsync();
            
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
                Dictionary<string, string> noResults = new Dictionary<string, string>
                {
                    { "NoResults", "No Results"}
                };

                return noResults;
            }
        }

        public static async Task<List<Dictionary<string, string>>> GetWeather(Dictionary<string, string> latLong, string weatherSelect)
        {
            List<Dictionary<string, string>> weatherList = new List<Dictionary<string, string>>();

            Dictionary<string, string> error = new Dictionary<string, string>
            {
                { "Error", "Connection Unsuccessful"}
            };

            Dictionary<string, string> noResults = new Dictionary<string, string>
            {
                { "NoResults", "No Results"}
            };
                        
                   
            if (latLong.ContainsKey("Error"))
            {
                weatherList.Add(error);
                return weatherList;
            }

            if (latLong.ContainsKey("NoResults"))
            {
                weatherList.Add(noResults);
                return weatherList;
            }            
            
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
                weatherList.Add(noResults);
                return weatherList;
            }
            
            JToken? xCord = token.SelectToken("gridX");
            JToken? yCord = token.SelectToken("gridY");
            JToken? gridID = token.SelectToken("gridId");

            HttpRequestMessage? forecastRequest;
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
                weatherList.Add(error);
                return weatherList;
            }

            HttpResponseMessage httpResponseForecast = await client.SendAsync(forecastRequest);

            if (!httpResponseForecast.IsSuccessStatusCode)
            {
                weatherList.Add(error);
                return weatherList;
            }

            string forecastResponse = await httpResponseForecast.Content.ReadAsStringAsync();

            JObject forecast = (JObject)JObject.Parse(forecastResponse)["properties"]!;
            JArray dailyForecast = (JArray)forecast["periods"]!;
            
            if (dailyForecast is null) 
            {
                weatherList.Add(noResults );
                return weatherList;
                //return new List<Dictionary<string, string>>();
            }
                        

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

                    dayWeather[$"DailyPrecipitation"] = dayWeather[$"DailyPrecipitation"] is null ? "0" : dayWeather[$"DailyPrecipitation"];
                                    
                    DateTime startTime;
                    DateTime endTime;
                    
                    if (!DateTime.TryParse(dayWeather["StartTime"], out startTime) || !DateTime.TryParse(dayWeather["EndTime"], out endTime))
                    {
                        dayWeather["Period"] = "INVALID";
                    }
                    else
                    {
                        dayWeather["Period"] = $"{startTime.DayOfWeek}  {startTime.Hour}:00 to {endTime.Hour}:00";
                    }
                    
                }
                weatherList.Add(dayWeather);
            }

            //return weatherList.Count == 0 ? new List<Dictionary<string, string>>() : weatherList;

            if (weatherList.Count == 0)
            {
                weatherList.Add(noResults);
                return weatherList; 
            }

            return weatherList;
        }
    }
}