using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Cache;
using System.Net.Http.Headers;
using Newtonsoft.Json;
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

            if (locationResponse == "[]")
            {
                Dictionary<string, string> invalid = new Dictionary<string, string>();
                return invalid;
            }

            string firstResponse = locationResponse.Split('{', '}')[1];
            firstResponse = "{" + firstResponse + "}";

            JObject jsonLocation = JObject.Parse(firstResponse);
            JToken? latitude = jsonLocation.SelectToken("lat");
            JToken? longitude = jsonLocation.SelectToken("lon");

            Dictionary<string, string> latLong = new Dictionary<string, string>
            {
                { "Latitude", (string)latitude },
                { "Longitude", (string) longitude }
            };

            return latLong;
        }

        public static async Task<List<Dictionary<string, string>>> GetWeather(Dictionary<string, string> latLong)
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
            var xCord = token.SelectToken("gridX");
            var yCord = token.SelectToken("gridY");
            var gridID = token.SelectToken("gridId");

            HttpRequestMessage forecastRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.weather.gov/gridpoints/{gridID}/{xCord},{yCord}/forecast");
            HttpResponseMessage httpResponseForecast = await client.SendAsync(forecastRequest);

            string forcastResponse = await httpResponseForecast.Content.ReadAsStringAsync();

            JObject forecast = (JObject)JObject.Parse(forcastResponse)["properties"]!;
            JArray dailyForecast = (JArray)forecast["periods"]!;

            if (dailyForecast == null)
            {
                return invalid;
            }

            List<Dictionary<string, string>> weatherList = new List<Dictionary<string, string>>();

            foreach (var day in dailyForecast)
            {
                Dictionary<string, string> dayWeather = new Dictionary<string, string>
                {
                    { $"Name", (string)day["name"] },
                    { $"Temp", (string)day["temperature"] },
                    { $"WindSpeed", (string)day["windSpeed"] },
                    { $"WindDirection", (string)day["windDirection"] },
                    { $"Humidity", (string)day["relativeHumidity"]["value"] },
                    { $"Dewpoint", (string)day["dewpoint"]["value"] },
                    { $"DailyPrecipitation", (string)day["probabilityOfPrecipitation"]["value"] },
                    { $"DetailedForecast", (string)day["detailedForecast"] }
                };

                if (dayWeather[$"DailyPrecipitation"] is null)
                {
                    dayWeather[$"DailyPrecipitation"] = "0";
                }

                weatherList.Add(dayWeather);
            }

            if (weatherList.Count == 0)
            {
                return invalid;
            }

            else
            {
                return weatherList;
            }
        }
    }
}